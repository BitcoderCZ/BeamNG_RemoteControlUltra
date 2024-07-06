using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected abstract bool dontDestroyOnLoad { get; }
        protected abstract bool destroyPrevious { get; }

        public static T Ins = null!;

        protected bool destroyed { get; private set; }

        protected void Awake()
        {
            if (Ins == null)
            {
                Ins = (T)this;
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (destroyPrevious)
                {
                    DestroyImmediate(Ins.gameObject);
                    Ins = (T)this;
                    if (dontDestroyOnLoad)
                        DontDestroyOnLoad(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                    destroyed = true;
                }
            }
        }

        private void OnDestroy()
        {
            if (Ins == this)
                Ins = null!;
        }
    }
}
