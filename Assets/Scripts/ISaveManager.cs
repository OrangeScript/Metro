using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * this interface needs to be inplemented for managers pursuing save and load
 */
public interface ISaveManager
{
    void LoadData(GameData _data);
    void SaveData(ref GameData _data);
}
