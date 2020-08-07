using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Audio;
using Managers;
using Settings;
using TMPro;
using UnityEngine;
using Utils;

namespace LevelObjects.DoorCode
{
    public class DoorCodeDial : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtDoorCodeDisplay;
        [SerializeField] private Door door;
        [SerializeField] private float wrongCodeTimer;

        [SerializeField] private List<DoorCodeButton> doorCodeButtons;

        private readonly LimitedList<int> _displayedNumbers = new LimitedList<int>(4);

        private bool _canPressButtons = true;

        private string _defaultDisplayText;
        private readonly Regex _regexUnderScore = new Regex("_");

        private readonly List<int> _doorCode = new List<int>();
        private int _currentCodeIndex;

        private void Awake()
        {
            foreach (DoorCodeButton doorCodeButton in doorCodeButtons)
                doorCodeButton.OnDoorCodeButtonPressed += OnDoorCodeButtonPressed;

            _defaultDisplayText = txtDoorCodeDisplay.text;

            for (int i = 0; i < 4; i++)
                _doorCode.Add(Random.Range(1, 10));
        }

        private void Start() => StartCoroutine(PlayDoorCode());

        private void OnDoorCodeButtonPressed(int buttonNumber)
        {
            if (!_canPressButtons)
                return;

            string newDisplayText = _regexUnderScore.Replace(txtDoorCodeDisplay.text, buttonNumber + " ", 1);
            txtDoorCodeDisplay.SetText(newDisplayText);
            _displayedNumbers.Add(buttonNumber);
            if (_displayedNumbers.IsFull)
                CheckCode();
        }

        private void CheckCode()
        {
            _canPressButtons = false;
            if (_displayedNumbers.Equals(_doorCode))
            {
                txtDoorCodeDisplay.color = Consts.Colors.GREEN_CORRECT;
                door.Activate();
            }
            else
            {
                txtDoorCodeDisplay.color = Consts.Colors.RED_WRONG;
                CoroutineManager.Instance.WaitForSeconds(wrongCodeTimer, () =>
                {
                    txtDoorCodeDisplay.color = Color.white;
                    txtDoorCodeDisplay.SetText(_defaultDisplayText);
                    _displayedNumbers.Clear();
                    _canPressButtons = true;
                });
            }
        }

        private IEnumerator PlayDoorCode()
        {
            if (_currentCodeIndex == _doorCode.Count)
            {
                _currentCodeIndex = 0;
                yield return new WaitForSeconds(2f);
            }

            AudioManager.Instance.PlayAudio((AudioEnum.Numbers) _doorCode[_currentCodeIndex] - 1);
            _currentCodeIndex++;
            yield return new WaitUntil(() => AudioManager.Instance.CanPlayAudio());
            yield return new WaitForSeconds(0.5f);
            yield return PlayDoorCode();
        }
    }
}