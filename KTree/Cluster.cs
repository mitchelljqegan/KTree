using KTreeDataTypes;

namespace KTree
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cluster<T> where T : KTreeDataType<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T Mean { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public KTreeKey<T>[] Observations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public KTreeKey<T>[] PreviousObservations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="maxObservations"></param>
        public Cluster(T mean, int maxObservations)
        {
            Mean = mean.Clone() as T;
            Observations = new KTreeKey<T>[maxObservations];
            PreviousObservations = new KTreeKey<T>[maxObservations];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kTreeKey"></param>
        public void AddKTreeKey(KTreeKey<T> kTreeKey)
        {
            for (int i = 0; i < Observations.Length; i++)
            {
                if (Observations[i] == default)
                {
                    Observations[i] = kTreeKey;
                    break;
                }
            }
        }

        /// <summary>
        /// Checks whether cluster assignments have changed since previous assignment.
        /// </summary>
        /// <returns></returns>
        public bool AssignmentChanged()
        {
            bool changed = false;

            for (int i = 0; i < Observations.Length; i++)
            {
                if (Observations[i] != default)
                {
                    if (Observations[i].CompareTo(PreviousObservations[i]) != 0)
                    {
                        changed = true;
                        break;
                    }
                }
                else if (PreviousObservations[i] != default)
                {
                    changed = true;
                    break;
                }
                else
                {
                    break;
                }

            }

            return changed;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearKeys()
        {
            for (int i = 0; i < Observations.Length; i++)
            {
                if (Observations[i] != default)
                {
                    PreviousObservations[i] = Observations[i].Clone() as KTreeKey<T>;
                }
                else
                {
                    PreviousObservations[i] = default;
                }

                Observations[i] = default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateMean()
        {
            int numKeys = 0;
            Mean.ClearProperties();

            foreach (KTreeKey<T> kTreeKey in Observations)
            {
                if (kTreeKey != default)
                {
                    Mean += kTreeKey.Key;
                    numKeys++;
                }
                else
                {
                    break;
                }
            }

            Mean /= numKeys;
        }
    }
}
