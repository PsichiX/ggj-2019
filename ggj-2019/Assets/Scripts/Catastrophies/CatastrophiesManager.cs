using UnityEngine;

namespace GaryMoveOut.Catastrophies
{
    public class CatastrophiesManager
    {
        private CatastrophiesDatabase catastrophiesDatabase;
        private float[] catastrophyProbabilities;

        public CatastrophiesManager()
        {
            catastrophiesDatabase = Resources.Load<CatastrophiesDatabase>("Databases/CatastrophiesDatabase");
            catastrophiesDatabase.LoadDataFromResources();

            var count = catastrophiesDatabase.database.Count;
            catastrophyProbabilities = new float[count];
            for(int i = 0; i < catastrophyProbabilities.Length; i++)
            {
                catastrophyProbabilities[i] = 1f / count;
            }
        }

        public BaseCatastrophy GetRandomCatastrophy()
        {
            BaseCatastrophy catastrophy = null;

            float sum = 0f;
            int i = 0;
            int index = 0;
            for(i = 0; i < catastrophyProbabilities.Length; i++)
            {
                sum += catastrophyProbabilities[i];
            }

            // choose next catastrophy:
            var rnd = UnityEngine.Random.Range(0f, sum);
            for(i = 0; i < catastrophyProbabilities.Length; i++)
            {
                rnd -= catastrophyProbabilities[i];
                if (rnd < 0)
                {
                    index = i;
                    break;
                }
            }

            // update probabilities:
            float dChance = catastrophyProbabilities[index] / catastrophyProbabilities.Length;
            for (i = 0; i < catastrophyProbabilities.Length; i++)
            {
                if (i == index)
                {
                    catastrophyProbabilities[i] = dChance;
                }
                else
                {
                    catastrophyProbabilities[i] += dChance;
                }
            }

            catastrophy = catastrophiesDatabase.database[index];
            return catastrophy;
        }
    }
}
