using UnityEngine;
using CompassNavigatorPro;

namespace CompassNavigatorProDemos {

    public class MiniMapScrollWheel : MonoBehaviour {

        public float zoomSpeed = 100f;

        CompassPro compass;
        bool canScrollWheel;

        void Start() {
            // Get a reference to the Compass Pro Navigator component
            compass = CompassPro.instance;

            // Subscribe to Compass minimap events
            compass.OnMiniMapMouseEnter += (Vector2 p) => canScrollWheel = true;
            compass.OnMiniMapMouseExit += (Vector2 p) => canScrollWheel = false;
        }

        void Update() {

            if (canScrollWheel) {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (scroll != 0) {
                    compass.MiniMapZoomIn(zoomSpeed * scroll);
                }
            }

        }


    }
}