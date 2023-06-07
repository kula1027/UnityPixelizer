using UnityEditor;
using UnityEngine;

namespace Pixelizer {
    public class Palettize_Sectionize : PixelizerMethod {
        [SerializeField] private int sectionCountXY;
        [SerializeField] private int sectionCountZ;

        public override void OnGUI() {
            int input;
            input = EditorGUILayout.IntField("sectionCountXY", sectionCountXY);
            sectionCountXY = input <= 0 ? 1 : input;

            input = EditorGUILayout.IntField("sectionCountZ", sectionCountZ);
            sectionCountZ = input <= 0 ? 1 : input;
        }

        public override Texture2D Process(PixelizerIn input) {
            return Pixelizer.Palettize(input.Texture, RoundOffNormal, input.AlphaCut);
        }

        private Color32 RoundOffNormal(Color32 color) {
            Vector3 normal = new Vector3(
                (color.r / 255f) * 2 - 1,
                (color.g / 255f) * 2 - 1,
                (color.b / 255f) * 2 - 1
            );

            //xy평면 위 / x+ -> y+
            float alpha_xy = Mathf.Atan2(normal.y, normal.x);//alpha
            alpha_xy = alpha_xy < 0 ? alpha_xy + Mathf.PI * 2 : alpha_xy;

            //z+축과의 각
            float beta_z = Mathf.Acos(normal.z);//beta
            beta_z = beta_z < 0 ? beta_z + Mathf.PI * 2 : beta_z;

            float divider_xy = Mathf.PI * 2f / sectionCountXY;
            float divider_z = Mathf.PI / 2 / sectionCountZ;
            float div_xy = alpha_xy / divider_xy;
            float div_z = beta_z / divider_z;

            float alpha_xy_sect = ((int)div_xy + .5f) * divider_xy;
            float beta_z_sect = ((int)div_z + .5f) * divider_z;

            Vector3 outNormal = new Vector3(
                 Mathf.Sin(beta_z_sect) * Mathf.Cos(alpha_xy_sect),
                 Mathf.Sin(beta_z_sect) * Mathf.Sin(alpha_xy_sect),
                 Mathf.Cos(beta_z_sect)
              ).normalized;

            return new Color(
               (outNormal.x + 1) * .5f,
               (outNormal.y + 1) * .5f,
               (outNormal.z + 1) * .5f,
               1
               );
        }
    }
}