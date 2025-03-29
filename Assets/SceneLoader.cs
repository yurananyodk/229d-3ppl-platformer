using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadCreditScene()
    {
        SceneManager.LoadScene("credit"); // ตรวจสอบให้ชื่อ Scene ตรงกัน
    }
}