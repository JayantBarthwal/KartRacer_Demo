using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
public class HomeScript : MonoBehaviour
{
    public CinemachineFreeLook freeCam;
    public float rotSpeed = 1f;
    public Animation KartAnim;//little jump animation
    bool camControl = true;
    public Material mat;
    public Texture[] tex;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        SetTexture();
    }
    public void changekartcolor(int id)
    {
        PlayerPrefs.SetInt("colorCode", id);
        KartAnim.Play("carColorChange");
        SetTexture();
    }
    void SetTexture()
    {
        int id = PlayerPrefs.GetInt("colorCode");
        mat.mainTexture = tex[id];
    }

    private void Update()
    {
        if (camControl)
        {
            freeCam.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
            freeCam.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
        }
        freeCam.m_XAxis.Value = rotSpeed;
    }
    public void StartRace() {
        SceneManager.LoadScene(1);
    }
    public void CamControlValue(bool x)=>camControl = x; 
}
