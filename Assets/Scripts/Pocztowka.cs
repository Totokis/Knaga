using UnityEngine;

public class Pocztowka : MonoBehaviour
{
    [SerializeField] private GameObject pocztowka;
    [SerializeField] private GameObject pocztowka2;
    [SerializeField] private GameObject pocztowka3;
    

    public void ShowPocztowka()
    {
        // Wybierz losowo jedną z pocztówek do aktywacji
        GameObject[] pocztowki = { pocztowka, pocztowka2, pocztowka3 };
        int index = Random.Range(0, pocztowki.Length);
        pocztowki[index].SetActive(true);
        // LeanTween.color(pocztowki[index], Color.white, .5f).setFromColor(Color.clear);
        LeanTween.alpha(pocztowki[index], 1f, 0.1f).setFrom(0f);
    }
}
