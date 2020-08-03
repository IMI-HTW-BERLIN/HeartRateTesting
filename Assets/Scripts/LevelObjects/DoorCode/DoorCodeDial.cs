using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace LevelObjects.DoorCode
{
    public class DoorCodeDial : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtDoorCodeDisplay;
        [SerializeField] private Door door;
        [SerializeField] private float wrongCodeTimer;

        [Header("4-Digit Door Code")] [SerializeField]
        private List<int> doorCode;

        [SerializeField] private List<DoorCodeButton> doorCodeButtons;

        private LimitedList<int> _displayedNumbers = new LimitedList<int>(4);

        private bool _canPressButtons = true;

        private string _defaultDisplayText;
        private Regex _regex = new Regex("_");

        private void Awake()
        {
            foreach (DoorCodeButton doorCodeButton in doorCodeButtons)
            {
                doorCodeButton.OnDoorCodeButtonPressed += OnDoorCodeButtonPressed;
            }

            _defaultDisplayText = txtDoorCodeDisplay.text;
        }

        private void OnDoorCodeButtonPressed(int buttonNumber)
        {
            string newDisplayText = _regex.Replace(txtDoorCodeDisplay.text, buttonNumber + " ", 1);
            txtDoorCodeDisplay.SetText(newDisplayText);
            _displayedNumbers.Add(buttonNumber);
            if (_displayedNumbers.IsFull)
                CheckCode();
        }

        private void CheckCode()
        {
            _canPressButtons = false;
            if (_displayedNumbers.Equals(doorCode))
            {
                txtDoorCodeDisplay.color = Consts.Colors.GREEN_CORRECT;
                door.Activate();
            }
            else
            {
                txtDoorCodeDisplay.color = Consts.Colors.RED_WRONG;
                StartCoroutine(WaitForSeconds(wrongCodeTimer, () =>
                {
                    txtDoorCodeDisplay.color = Color.white;
                    txtDoorCodeDisplay.SetText(_defaultDisplayText);
                    _displayedNumbers.Clear();
                    _canPressButtons = true;
                }));
            }
        }

        private IEnumerator WaitForSeconds(float time, UnityAction onFinish)
        {
            yield return new WaitForSeconds(time);
            onFinish.Invoke();
        }
    }
}