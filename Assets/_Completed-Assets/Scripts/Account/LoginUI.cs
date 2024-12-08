using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    // Start is called before the first frame update
    public void ToUserReistration() {

        SceneManager.LoadScene(SceneNames.UserReistration);
    
    }

    public void ToRanking() {

        SceneManager.LoadScene(SceneNames.Ranking);
    }

    public void ToStart() {

        SceneManager.LoadScene(SceneNames.GameScene);


    }
    
}
