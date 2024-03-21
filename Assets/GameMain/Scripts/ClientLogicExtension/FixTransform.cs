/*
* 文件名：FixTransform
* 文件描述：
* 作者：aronliang
* 创建时间：2023/07/28 19:16:06
* 修改记录：
*/

using LogicShared.TrueSync.Math;
using UnityEngine;

namespace GameMain
{
    public class FixTransform: MonoBehaviour
    {
        private FixVector _position;
        public FixVector position {
            get => _position;
            set => _position = value;
        }
        
        private FixQuaternion _rotation;
        public FixQuaternion rotation {
            get => _rotation;
            set => _rotation = value;
        }

        private FixVector _scale;
        public FixVector scale {
            get => _scale;
            set => _scale = value;
        }
        
         public void LookAt(FixTransform other) {
            LookAt(other.position);
        }
        
        public void LookAt(FixVector target) {
            rotation = FixQuaternion.CreateFromMatrix(FixMatrix.CreateFromLookAt(position, target));
        }
        
        public void Translate(Fix64 x, Fix64 y, Fix64 z) {
            Translate(x, y, z, Space.World);
        }
        
        public void Translate(Fix64 x, Fix64 y, Fix64 z, Space relativeTo) {
            Translate(new FixVector(x, y, z), relativeTo);
        }
        
        public void Translate(Fix64 x, Fix64 y, Fix64 z, FixTransform relativeTo) {
            Translate(new FixVector(x, y, z), relativeTo);
        }
        
        public void Translate(FixVector translation) {
            Translate(translation, Space.Self);
        }
        
        public void Translate(FixVector translation, Space relativeTo) {
            if (relativeTo == Space.Self) {
                Translate(translation, this);
            } else {
                this.position += translation;
            }
        }
        
        public void Translate(FixVector translation, FixTransform relativeTo) {
            this.position += FixVector.Transform(translation, FixMatrix.CreateFromQuaternion(relativeTo.rotation));
        }
        
        public void RotateAround(FixVector point, FixVector axis, Fix64 angle) {
            FixVector vector = this.position;
            FixVector vector2 = vector - point;
            vector2 = FixVector.Transform(vector2, FixMatrix.AngleAxis(angle * Fix64.Deg2Rad, axis));
            vector = point + vector2;
            this.position = vector;

            Rotate(axis, angle);
        }
        
        public void RotateAround(FixVector axis, Fix64 angle) {
            Rotate(axis, angle);
        }
        
        public void Rotate(Fix64 xAngle, Fix64 yAngle, Fix64 zAngle) {
            Rotate(new FixVector(xAngle, yAngle, zAngle), Space.Self);
        }
        
        public void Rotate(Fix64 xAngle, Fix64 yAngle, Fix64 zAngle, Space relativeTo) {
            Rotate(new FixVector(xAngle, yAngle, zAngle), relativeTo);
        }
        
        public void Rotate(FixVector eulerAngles) {
            Rotate(eulerAngles, Space.Self);
        }
        
        public void Rotate(FixVector axis, Fix64 angle) {
            Rotate(axis, angle, Space.Self);
        }
        
        public void Rotate(FixVector axis, Fix64 angle, Space relativeTo) {
            FixQuaternion result = FixQuaternion.identity;

            if (relativeTo == Space.Self) {
                result = this.rotation * FixQuaternion.AngleAxis(angle, axis);
            } else {
                result = FixQuaternion.AngleAxis(angle, axis) * this.rotation;
            }

            result.Normalize();
            this.rotation = result;
        }
        
        public void Rotate(FixVector eulerAngles, Space relativeTo) {
            FixQuaternion result = FixQuaternion.identity;

            if (relativeTo == Space.Self) {
                result = this.rotation * FixQuaternion.Euler(eulerAngles);
            } else {
                result = FixQuaternion.Euler(eulerAngles) * this.rotation;
            }

            result.Normalize();
            this.rotation = result;
        }
        
        public FixVector forward {
            get {
                return FixVector.Transform(FixVector.forward, FixMatrix.CreateFromQuaternion(rotation));
            }
        }
        
        public FixVector right {
            get {
                return FixVector.Transform(FixVector.right, FixMatrix.CreateFromQuaternion(rotation));
            }
        }
        
        public FixVector up {
            get {
                return FixVector.Transform(FixVector.up,FixMatrix.CreateFromQuaternion(rotation));
            }
        }
        
        public FixVector eulerAngles {
            get {
                return rotation.eulerAngles;
            }
        }

        public void UpdateTransform(Transform transform) {
            transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
            //transform.rotation = new Quaternion((float)rotation.x, (float)rotation.y, (float)rotation.z, (float)rotation.w);
            //transform.localScale = new Vector3((float)scale.x, (float)scale.y, (float)scale.z);
        }
    }
}