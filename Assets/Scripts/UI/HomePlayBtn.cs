using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HomePlayBtn : FUButton
{
    public override void DoSomething()
    {
        SceneManager.LoadScene(Consts.PlayMenu);
    }
}
