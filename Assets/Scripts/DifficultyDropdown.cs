using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDropdown : MonoBehaviour
{
  public Dropdown dd;
  public GameObject explanation;
  private void Start()
  {
    dd.value = Manager.CPUDifficulty;
  }

  public void DropDownValueChanged(Dropdown dd)
  {
    Manager.CPUDifficulty = dd.value;
    if (dd.value == 1)
    {
      Manager.CPUAngle = 60;
      explanation.SetActive(true);
    }
    else
    {
      Manager.CPUAngle = 30;
      explanation.SetActive(false);
    }
  }
}
