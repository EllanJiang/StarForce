using GameFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace StarForce.Editor.Profiler
{
    internal sealed class ProfilerSamplingInjection
    {
        private const string GameFrameworkAssemblyName = "GameFramework.dll";
        private const string EditorPrefsKey = "StarForce.Editor.Profiler.ProfilerSamplingInjection.Enable";

        private static readonly string[] AssemblyNamesToInjection = new string[]
        {
            "GameFramework.dll",
            "Assembly-CSharp-firstpass.dll",
            "Assembly-CSharp.dll",
            "Assembly-UnityScript-firstpass.dll",
            "Assembly-UnityScript.dll",
        };

        [MenuItem("Star Force/Profiler Sampling Injection/Enable")]
        private static void EnableProfilerSamplingInjection()
        {
            EditorPrefs.SetBool(EditorPrefsKey, true);
        }

        [MenuItem("Star Force/Profiler Sampling Injection/Enable", true)]
        private static bool EnableProfilerSamplingInjectionValidate()
        {
            return !EditorPrefs.GetBool(EditorPrefsKey, false);
        }

        [MenuItem("Star Force/Profiler Sampling Injection/Disable")]
        private static void DisableProfilerSamplingInjection()
        {
            EditorPrefs.SetBool(EditorPrefsKey, false);
        }

        [MenuItem("Star Force/Profiler Sampling Injection/Disable", true)]
        private static bool DisableProfilerSamplingInjectionValidate()
        {
            return EditorPrefs.GetBool(EditorPrefsKey, false);
        }

        [PostProcessBuild(0)]
        private static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (!EditorPrefs.GetBool(EditorPrefsKey, false))
            {
                return;
            }

            if (target != BuildTarget.Android)
            {
                return;
            }

            // 如果直接打成了 APK 包，而非 Android 工程，则不再执行。
            if (pathToBuiltProject.ToLower().EndsWith(".apk"))
            {
                return;
            }

            string assemblyPath = Utility.Path.GetCombinePath(pathToBuiltProject, PlayerSettings.productName, "assets/bin/Data/Managed");
            InjectProfilerSampling(assemblyPath, AssemblyNamesToInjection);
        }

        private static bool InjectProfilerSampling(string assemblyPath, string[] assemblyNames)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                Debug.LogWarning("Assembly path is invalid.");
                return false;
            }

            if (assemblyNames == null || assemblyNames.Length <= 0)
            {
                Debug.LogWarning("Assembly names are invalid.");
                return false;
            }

            MethodDefinition beginSampleMethod = null;
            MethodDefinition endSampleMethod = null;
            if (!PrepareProfilerSamplingInjectionMethods(assemblyPath, out beginSampleMethod, out endSampleMethod))
            {
                Debug.LogWarning("Prepare profiler sampling injection methods failure.");
                return false;
            }

            try
            {
                Injection injection = new Injection(beginSampleMethod.Resolve(), endSampleMethod.Resolve());
                injection.ProgressChanged += delegate (string assemblyName, float progress)
                {
                    EditorUtility.DisplayProgressBar("Profiler Sampling Injection", string.Format("Injecting assembly {0}, {1} completed...", assemblyName, progress.ToString("P1")), progress);
                };

                foreach (string assemblyName in assemblyNames)
                {
                    injection.RunInject(assemblyPath, assemblyName);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning(exception.ToString());
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return true;
        }

        private static bool PrepareProfilerSamplingInjectionMethods(string assemblyPath, out MethodDefinition beginSampleMethod, out MethodDefinition endSampleMethod)
        {
            beginSampleMethod = null;
            endSampleMethod = null;

            if (string.IsNullOrEmpty(assemblyPath))
            {
                Debug.LogWarning("Assembly path is invalid.");
                return false;
            }

            string gameFrameworkAssemblyPath = Utility.Path.GetCombinePath(assemblyPath, GameFrameworkAssemblyName);
            if (!File.Exists(gameFrameworkAssemblyPath))
            {
                Debug.LogWarning("Can not find GameFramework.dll.");
                return false;
            }

            ModuleDefinition gameFrameworkModuleDefinition = ModuleDefinition.ReadModule(gameFrameworkAssemblyPath);
            if (gameFrameworkModuleDefinition == null)
            {
                Debug.LogWarning("Can not load GameFramework.dll.");
                return false;
            }

            TypeDefinition profilerTypeDefinition = gameFrameworkModuleDefinition.Types.SingleOrDefault(t => t.FullName == "GameFramework.Utility.Profiler");
            if (profilerTypeDefinition == null)
            {
                Debug.LogWarning("Can not find type GameFramework.Utility.Profiler in GameFramework.dll.");
                return false;
            }

            beginSampleMethod = profilerTypeDefinition.Methods.SingleOrDefault(m => m.FullName == "GameFramework.Utility.Profiler.BeginSample" && m.Parameters.Count == 1 && m.Parameters[0].ParameterType.FullName == "System.String");
            if (beginSampleMethod == null)
            {
                Debug.LogWarning("Can not find method BeginSample in GameFramework.Utility.Profiler.");
                return false;
            }

            endSampleMethod = profilerTypeDefinition.Methods.SingleOrDefault(m => m.FullName == "GameFramework.Utility.Profiler.EndSample" && m.Parameters.Count == 0);
            if (endSampleMethod == null)
            {
                Debug.LogWarning("Can not find method EndSample in GameFramework.Utility.Profiler.");
                return false;
            }

            return true;
        }

        private sealed class Injection
        {
            private static readonly HashSet<OpCode> BranchOpCodes = new HashSet<OpCode>
            {
                OpCodes.Beq,
                OpCodes.Beq_S,
                OpCodes.Bge,
                OpCodes.Bge_S,
                OpCodes.Bge_Un,
                OpCodes.Bge_Un_S,
                OpCodes.Ble,
                OpCodes.Ble_S,
                OpCodes.Ble_Un,
                OpCodes.Ble_Un_S,
                OpCodes.Blt,
                OpCodes.Blt_S,
                OpCodes.Blt_Un,
                OpCodes.Blt_Un_S,
                OpCodes.Bne_Un,
                OpCodes.Bne_Un_S,
                OpCodes.Br,
                OpCodes.Br_S,
                OpCodes.Brfalse,
                OpCodes.Brfalse_S,
                OpCodes.Brtrue,
                OpCodes.Brtrue_S,
            };

            private MethodDefinition m_BeginSampleMethod;
            private MethodDefinition m_EndSampleMethod;

            public event GameFrameworkAction<string, float> ProgressChanged = null;

            public Injection(MethodDefinition beginSampleMethod, MethodDefinition endSampleMethod)
            {
                m_BeginSampleMethod = beginSampleMethod;
                m_EndSampleMethod = endSampleMethod;
            }

            public void RunInject(string assemblyPath, string assemblyName)
            {
                OnProgressChanged(assemblyName, 0f);

                string fullName = Utility.Path.GetCombinePath(assemblyPath, assemblyName);
                if (!File.Exists(fullName))
                {
                    Debug.LogWarning(string.Format("Assembly '{0}' does not exist.", fullName));
                    return;
                }

                DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory(Path.GetDirectoryName(fullName));

                ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(fullName, new ReaderParameters { AssemblyResolver = resolver });
                Collection<TypeDefinition> typeDefinitions = moduleDefinition.Types;
                int typeCount = typeDefinitions.Count;
                if (typeCount <= 0)
                {
                    return;
                }

                int index = 0;
                foreach (TypeDefinition typeDefinition in typeDefinitions)
                {
                    if (NeedInject(typeDefinition))
                    {
                        Collection<MethodDefinition> methodDefinitions = typeDefinition.Methods;
                        foreach (MethodDefinition methodDefinition in methodDefinitions)
                        {
                            if (NeedInject(methodDefinition))
                            {
                                InjectMethod(typeDefinition, methodDefinition);
                            }
                        }
                    }

                    OnProgressChanged(assemblyName, ++index / (float)typeCount);
                }

                moduleDefinition.Write(fullName, new WriterParameters { WriteSymbols = true });
            }

            private static bool NeedInject(TypeDefinition typeDefinition)
            {
                if (typeDefinition == null || !typeDefinition.HasMethods || typeDefinition.IsEnum || typeDefinition.IsAutoClass || typeDefinition.IsAbstract)
                {
                    return false;
                }

                string dontProfileAttributeFullName = typeof(DontProfileAttribute).FullName;
                foreach (ICustomAttribute customAttribute in typeDefinition.CustomAttributes)
                {
                    if (customAttribute.AttributeType.FullName == dontProfileAttributeFullName)
                    {
                        return false;
                    }
                }

                return true;
            }

            private static bool NeedInject(MethodDefinition methodDefinition)
            {
                if (methodDefinition == null || !methodDefinition.HasBody || methodDefinition.IsAbstract || methodDefinition.IsConstructor)
                {
                    return false;
                }

                foreach (ICustomAttribute customAttribute in methodDefinition.CustomAttributes)
                {
                    string dontProfileAttributeFullName = typeof(DontProfileAttribute).FullName;
                    if (customAttribute.AttributeType.FullName == dontProfileAttributeFullName)
                    {
                        return false;
                    }
                }

                return true;
            }

            private void InjectMethod(TypeDefinition targetType, MethodDefinition methodDefinition)
            {
                var instructions = methodDefinition.Body.Instructions;
                var loadStr = Instruction.Create(OpCodes.Ldstr, string.Format("[{0}.{1}]", targetType.Name, methodDefinition.Name));
                instructions.Insert(0, loadStr);
                var callBegin = Instruction.Create(OpCodes.Call, methodDefinition.Module.ImportReference(m_BeginSampleMethod));
                instructions.Insert(1, callBegin);

                var jumpInstructions = GetJumpInstructions(instructions);
                for (int i = 0; i < instructions.Count; i++)
                {
                    var instruction = instructions[i];
                    if (instruction.OpCode != OpCodes.Ret)
                    {
                        continue;
                    }

                    var callEnd = Instruction.Create(OpCodes.Call, methodDefinition.Module.ImportReference(m_EndSampleMethod));
                    instructions.Insert(i, callEnd);

                    var jumpInstructionsToRemove = new HashSet<Instruction>();
                    foreach (var jumpInstruction in jumpInstructions)
                    {
                        var targetInstruction = jumpInstruction.Operand as Instruction;
                        if (targetInstruction == null)
                        {
                            Debug.LogWarning(string.Format("Weird! Branch statement doesn't have target instruction in '{0}.{1}'.", targetType.FullName, methodDefinition.Name));
                            continue;
                        }

                        if (targetInstruction == instruction)
                        {
                            jumpInstruction.Operand = callEnd;
                            jumpInstructionsToRemove.Add(jumpInstruction);
                        }
                    }

                    jumpInstructions.RemoveWhere(ins => jumpInstructionsToRemove.Contains(ins));

                    // 新增了一条指令，所以迭代子 i 需要多增加一次。
                    i++;
                }
            }

            private HashSet<Instruction> GetJumpInstructions(Collection<Instruction> instructions)
            {
                HashSet<Instruction> jumpInstructions = new HashSet<Instruction>();
                for (int i = 0; i < instructions.Count; i++)
                {
                    Instruction instruction = instructions[i];
                    if (!BranchOpCodes.Contains(instruction.OpCode))
                    {
                        continue;
                    }

                    jumpInstructions.Add(instruction);
                }

                return jumpInstructions;
            }

            private void OnProgressChanged(string assemblyName, float progress)
            {
                if (ProgressChanged == null)
                {
                    return;
                }

                ProgressChanged(assemblyName, progress);
            }
        }
    }
}
