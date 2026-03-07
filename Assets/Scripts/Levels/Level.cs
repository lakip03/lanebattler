using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "levelMaker/level-object")]
public class Level : ScriptableObject
{
    public int levelNumber;
    public List<Wave> waves;

    public Wave GetWaveByWaveNumber(int number) => waves.Find(w => w.waveNumber == number);
    public int GetTotalNumberOfWaves() => waves.Count;
}