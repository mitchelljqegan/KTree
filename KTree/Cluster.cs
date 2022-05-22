using KTreeDataTypes;
using System.Collections.Generic;

namespace KTree
{
    public class Cluster<T> where T : KTreeDataType<T>
    {
        public T Mean { get; set; }

        public List<KTreeKey<T>> Observations { get; set; }

        public List<KTreeKey<T>> PreviousObservations { get; set; }

        public Cluster(T mean, int maxObservations)
        {
            Mean = mean.Clone() as T;
            Observations = new(maxObservations);
            PreviousObservations = new();
        }

        public void AddKTreeKey(KTreeKey<T> kTreeKey)
        {
            Observations.Add(kTreeKey);
        }

        /// <summary>
        /// Checks whether cluster assignments have changed since previous assignment.
        /// </summary>
        /// <returns></returns>
        public bool AssignmentChanged()
        {
            if (Observations.Count != PreviousObservations.Count)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < Observations.Count; i++)
                {
                    if (Observations[i].CompareTo(PreviousObservations[i]) != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void ClearKeys()
        {
            PreviousObservations.Clear();

            foreach (KTreeKey<T> observation in Observations)
            {
                PreviousObservations.Add(observation.Clone() as KTreeKey<T>);
            }

            Observations.Clear();
        }

        public void UpdateMean()
        {
            Mean.ClearProperties();

            foreach (KTreeKey<T> kTreeKey in Observations)
            {
                Mean += kTreeKey.Key;
            }

            Mean /= Observations.Count;
        }
    }
}
