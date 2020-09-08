using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MinesweeperGrid grid;
    [SerializeField] private TileClickProcessor userInteraction;

    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Text endGameText;

    [Header("Easy settings")]
    [SerializeField] private int easySize;
    [SerializeField] private float easyBombPercentage;

    [Header("Medium settings")]
    [SerializeField] private int mediumSize;
    [SerializeField] private float mediumBombPercentage;

    [Header("Hard settings")]
    [SerializeField] private int hardSize;
    [SerializeField] private float hardBombPercentage;

    private Coroutine showEndGamePanelDelayedCoroutine = null;

    private void Awake() {
        StartEasyGame();
    }

    public void StartEasyGame() {
        StartGame(easySize, easyBombPercentage);
    }

    public void StartMediumGame() {
        StartGame(mediumSize, mediumBombPercentage);
    }

    public void StartHardGame() {
        StartGame(hardSize, hardBombPercentage);
    }

    public void StartGame(int size, float percentageBomb) {
        if (showEndGamePanelDelayedCoroutine != null) {
            StopCoroutine(showEndGamePanelDelayedCoroutine);
            showEndGamePanelDelayedCoroutine = null;
        }

        grid.Initialize(size, percentageBomb);
        userInteraction.Initialize();
        endGamePanel.SetActive(false);
    }

    public void EndGame(bool hasWon) {
        showEndGamePanelDelayedCoroutine = StartCoroutine(ShowEndGamePanelDelayed());

        if (hasWon) {
            endGameText.text = "Brilliant !";
        } else {
            endGameText.text = "That's unfortunate...";
        }
    }

    private IEnumerator ShowEndGamePanelDelayed() {
        yield return new WaitForSeconds(0.5f);
        endGamePanel.SetActive(true);
    }
}
