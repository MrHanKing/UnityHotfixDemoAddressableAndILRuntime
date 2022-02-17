using UnityEngine;
using System;
using UnityEngine.UI;

namespace ETModel
{
    public class AxisConverterHelp
    {
        
        /// <summary>
        /// 2点的角度
        /// </summary>
        /// <param name="target"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        public static float AngleTwoPoint(Vector3 target, Vector3 effect)
        {
            return (float)(Math.Atan2(target.y - effect.y, target.x - effect.x)) * 180 / Mathf.PI;
        }

        /// <summary>
        /// 返回2点之间的距离
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static int GetDisTwoPoint(Vector2 pos1,Vector2 pos2) 
        {
            float aF = (pos2.x - pos1.x) * (pos2.x - pos1.x);
            float bF = (pos2.y - pos1.y) * (pos2.y - pos1.y);

            return (int)Mathf.Sqrt(aF + bF);
        }

        /// <summary>
        ///  鼠标的射线到三维场景  拖3D物体跟随鼠标
        /// </summary>
        /// <returns></returns>
        public static Vector3 ObjectAxisTargetMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                return hitInfo.point;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// UGUI物体 跟随鼠标移动 pos坐标  
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static Vector3 UguiAxisTargetMousePos(Canvas canvas)
        {
            Vector3 pos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos))
            {

                return pos;
            }
            return Vector3.zero;
        }
        /// <summary>
        /// UGUI物体 跟随鼠标移动 Local坐标  
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static Vector2 UguiAxisTargetMouseLocalPos(Canvas canvas)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos))
            {

                return pos;
            }
            return Vector2.zero;
        }
        public static Vector2 UguiAxisTargetLocalPos(Canvas canvas,Vector3 vector)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, vector, canvas.worldCamera, out pos))
            {

                return pos;
            }
            return Vector2.zero;
        }
        public static Vector3 UguiAxisTargetObjPos(Canvas canvas, GameObject go)
        {
            Vector3 pos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, go.transform.position, canvas.worldCamera, out pos))
            {

                return pos;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// //计算图片中心和鼠标点的差值
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static Vector2 UguiAxisTargetMouseOffset(Canvas canvas, GameObject go)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos))
            {

                return go.GetComponent<RectTransform>().anchoredPosition - pos;
            }
            return Vector2.zero;
        }

        public static Vector2 UguiAxisTargetMouseLocalPos(Canvas canvas, GameObject go)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, go.transform.position, canvas.worldCamera, out pos))
            {

                return pos;
            }
            return Vector2.zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="cam"></param>
        /// <param name="father">最高级的父节点</param>
        /// <returns></returns>
        //是否摄像机内图片
        public static bool GetBgView(Transform pos, Transform cam, Transform father = null)
        {

            //摄像机 坐标
            float camx = cam.transform.localPosition.x;
            float camy = cam.transform.localPosition.y;
            //可视物体坐标
            float objx = pos.localPosition.x;
            float objy = pos.localPosition.y;

            if (father != null)
            {
                objx += father.localPosition.x;
                objy += father.localPosition.y;
                objx += father.parent.localPosition.x;
                objy += father.parent.localPosition.y;

            }
            else
            {
                objx = pos.localPosition.x;
                objy = pos.localPosition.y;
            }
            //屏幕长宽 物体长宽
            float width = Screen.width + pos.GetComponent<RectTransform>().sizeDelta.x / 2;
            float high = Screen.height + pos.GetComponent<RectTransform>().sizeDelta.y / 2;

            if (Mathf.Abs(objx - camx) >= width || Mathf.Abs(objy - camy) >= high)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        ///物体是否在相机前
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="camera"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public static bool IsInView(Transform pos,Camera camera,int offsetX = 0,int offsetY = 0)
        {
            //屏幕长宽 物体长宽
            float width =  pos.GetComponent<RectTransform>().sizeDelta.x;
            float high =  pos.GetComponent<RectTransform>().sizeDelta.y;
            Transform camTransform = camera.transform;

            Vector2 viewPos = camera.WorldToViewportPoint(pos.position);

            Vector3 dir = (pos.position - camTransform.position).normalized;

            float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面
            float disX = width + offsetX;
            float disY = high + offsetY;
            if (dot > 0 && Math.Abs((0.5f -viewPos.x) * width) <= disX && Math.Abs((0.5f - viewPos.y) * high) <= disY)

                return true;

            else

                return false;

        }

        //矩形碰撞
        public static bool RectangularCollision(Transform pos, Transform cam, Transform father = null)
        {
            //摄像机 坐标
            float camx = cam.transform.localPosition.x;
            float camy = cam.transform.localPosition.y;
            //可视物体坐标
            float objx = 0;
            float objy = 0;

            if (father != null)
            {
                objx = pos.localPosition.x + father.localPosition.x;
                objy = pos.localPosition.y + father.localPosition.y;
            }
            else
            {
                objx = pos.localPosition.x;
                objy = pos.localPosition.y;
            }
            //屏幕长宽 物体长宽
            float width = cam.GetComponent<RectTransform>().sizeDelta.x / 2 + pos.GetComponent<RectTransform>().sizeDelta.x / 2;
            float high = cam.GetComponent<RectTransform>().sizeDelta.y / 2 + pos.GetComponent<RectTransform>().sizeDelta.y / 2;

            if (Mathf.Abs(objx - camx) >= width || Mathf.Abs(objy - camy) >= high)
            {
                return false;
               
            }
            else
            {
                return true;
                
            }
        }

        //获取两直线交点
        public static Vector3 BeeLingMeet(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Vector3 Jiaod = Vector3.zero;
            float P1x = 0.0f;
            float P1y = 0.0f;
            float P1z = 0.0f;
            double plr1_x = p2.x - p1.x;
            double plr1_y = p2.y - p1.y;
            double plr1_z = p2.z - p1.z;
            double plr2_x = p4.x - p3.x;
            double plr2_y = p4.y - p3.y;
            double plr2_z = p4.z - p3.z;
            double t = 1.0f;
            if (((plr1_x != 0) && (plr2_x == 0)) || ((plr1_x == 0) && (plr2_x != 0)))
            {
                if (plr2_x == 0)
                {
                    t = (p3.x - p1.x) / plr1_x;
                    P1x = (float)(p1.x + t * plr1_x);
                    P1y = (float)(p1.y + t * plr1_y);
                    P1z = (float)(p1.z + t * plr1_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
                else
                {
                    t = (p1.x - p3.x) / plr2_x;
                    P1x = (float)(p3.x + t * plr2_x);
                    P1y = (float)(p3.y + t * plr2_y);
                    P1z = (float)(p3.z + t * plr2_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
            }
            else if (((plr1_y != 0) && (plr2_y == 0)) || ((plr1_y == 0) && (plr2_y != 0)))
            {
                if (plr2_y == 0)
                {
                    t = (p3.y - p1.y) / plr1_y;
                    P1x = (float)(p1.x + t * plr1_x);
                    P1y = (float)(p1.y + t * plr1_y);
                    P1z = (float)(p1.z + t * plr1_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
                else
                {
                    t = (p1.y - p3.y) / plr2_y;
                    P1x = (float)(p3.x + t * plr2_x);
                    P1y = (float)(p3.y + t * plr2_y);
                    P1z = (float)(p3.z + t * plr2_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
            }
            else if (((plr1_z != 0) && (plr2_z == 0)) || ((plr1_z == 0) && (plr2_z != 0)))
            {
                if (plr2_z == 0)
                {
                    t = (p3.z - p1.z) / plr1_z;
                    P1x = (float)(p1.x + t * plr1_x);
                    P1y = (float)(p1.y + t * plr1_y);
                    P1z = (float)(p1.z + t * plr1_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
                else
                {
                    t = (p1.z - p3.z) / plr2_z;
                    P1x = (float)(p3.x + t * plr2_x);
                    P1y = (float)(p3.y + t * plr2_y);
                    P1z = (float)(p3.z + t * plr2_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
            }
            else
            {
                if (((plr1_x != 0) && (plr2_x != 0)) && ((plr1_y != 0) && (plr2_y != 0)))
                {
                    double fz = (p3.x * plr2_y - p3.y * plr2_x - plr2_y * p1.x + plr2_x * p1.y);
                    double fm = (plr1_x * plr2_y - plr1_y * plr2_x);
                    t = fz / fm;
                    P1x = (float)(p1.x + t * plr1_x);
                    P1y = (float)(p1.y + t * plr1_y);
                    P1z = (float)(p1.z + t * plr1_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
                else if (((plr1_x != 0) && (plr2_x != 0)) && ((plr1_z != 0) && (plr2_z != 0)))
                {
                    double fz = (p3.x * plr2_z - p3.z * plr2_x - plr2_z * p1.x + plr2_x * p1.z);
                    double fm = (plr1_x * plr2_z - plr1_z * plr2_x);
                    t = fz / fm;
                    P1x = (float)(p1.x + t * plr1_x);
                    P1y = (float)(p1.y + t * plr1_y);
                    P1z = (float)(p1.z + t * plr1_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
                else if (((plr1_y != 0) && (plr2_y != 0)) && ((plr1_z != 0) && (plr2_z != 0)))
                {
                    double fz = (p3.y * plr2_z - p3.z * plr2_y - plr2_z * p1.y + plr2_y * p1.z);
                    double fm = (plr1_y * plr2_z - plr1_z * plr2_y);
                    t = fz / fm;
                    P1x = (float)(p1.x + t * plr1_x);
                    P1y = (float)(p1.y + t * plr1_y);
                    P1z = (float)(p1.z + t * plr1_z);
                    Jiaod = new Vector3(P1x, P1y, P1z);
                    return Jiaod;
                }
                else
                {
                    return Vector3.zero;
                }

            }
        }

        //判断两条线段是否相交
        public static bool GetIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {

            Vector3 intersection = Vector3.zero;

            //判断异常
            if (Math.Abs(b.x - a.y) + Math.Abs(b.x - a.x) + Math.Abs(d.x - c.x) + Math.Abs(d.x - c.x) == 0)
            {

                return false;
            }

            if (Math.Abs(b.y - a.y) + Math.Abs(b.x - a.x) == 0)
            {
                return false;
            }
            if (Math.Abs(d.y - c.y) + Math.Abs(d.x - c.x) == 0)
            {
                return false;
            }

            if ((b.y - a.y) * (c.x - d.x) - (b.x - a.x) * (c.y - d.y) == 0)
            {
                return false;
            }

            intersection.x = ((b.x - a.x) * (c.x - d.x) * (c.y - a.y) - c.x * (b.x - a.x) * (c.y - d.y) + a.x * (b.y - a.y) * (c.x - d.x)) / ((b.y - a.y) * (c.x - d.x) - (b.x - a.x) * (c.y - d.y));
            intersection.y = ((b.y - a.y) * (c.y - d.y) * (c.x - a.x) - c.y * (b.y - a.y) * (c.x - d.x) + a.y * (b.x - a.x) * (c.y - d.y)) / ((b.x - a.x) * (c.y - d.y) - (b.y - a.y) * (c.x - d.x));

            if ((intersection.x - a.x) * (intersection.x - b.x) <= 0 && (intersection.x - c.x) * (intersection.x - d.x) <= 0 && (intersection.y - a.y) * (intersection.y - b.y) <= 0 && (intersection.y - c.y) * (intersection.y - d.y) <= 0)
            {
                return true; //'相交
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断点是否在椭圆内
        /// </summary>
        /// <param name="cPoint"></param> 中心点
        /// <param name="pos"></param> 判断点
        /// <param name="more"></param>长轴
        /// <param name="less"></param>短轴
        public static bool GetPointInEllipse(Vector2 cPoint, Vector2 pos, float more, float less)
        {
            //中心点
            float x = cPoint.x;
            float y = cPoint.y;
            //判断点
            float px = pos.x;
            float py = pos.y;

            float num = (px - x) * (px - x) / (more * more) + (py - y) * (py - y) / (less * less);
            //Debug.Log("数值 = " + num);
            if (num <= 1)
                return true;

            return false;
        }


        /** 
         * 检测两个矩形是否碰撞 
         * @return 
         */
        public static bool isCollisionWithRect(int x1, int y1, int w1, int h1,
                int x2, int y2, int w2, int h2)
        {
            if (x1 >= x2 && x1 >= x2 + w2)
            {
                return false;
            }
            else if (x1 <= x2 && x1 + w1 <= x2)
            {
                return false;
            }
            else if (y1 >= y2 && y1 >= y2 + h2)
            {
                return false;
            }
            else if (y1 <= y2 && y1 + h1 <= y2)
            {
                return false;
            }
            return true;
        }

        /** 
         *  
         * @param x1 点 
         * @param y1 点 
         * @param x2 矩形view x 
         * @param y2 矩形view y 
         * @param w  矩形view 宽 
         * @param h  矩形view 高 
         * @return 
         */
        public static bool isCollision(int x1, int y1, int x2, int y2, int w, int h)
        {
            if (x1 >= x2 && x1 <= x2 + w && y1 >= y2 && y1 <= y2 + h)
            {
                return true;
            }
            return false;
        }

        public static bool IsAndroidPad()
        {
            float physicscreen = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) / Screen.dpi;
            if (physicscreen >= 7f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

