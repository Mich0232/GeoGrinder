namespace Moitho.Map
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MapPoint : MonoBehaviour, IClickable
    {
        public GameObject prefab;
        public PointType pointType;
        public string label;

        public virtual void OnPlayerClick() { }
    }

    public interface IClickable
    {
        void OnPlayerClick();
    }

    public enum PointType
    {
        culture,
        sport,
        food,
        other
    }
}
