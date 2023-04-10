using UnityEngine;
using TMPro;
using System;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public TextMeshPro gameOverText;
    public TextMeshPro scoreText;
    public TextMeshPro livesText;
    public TextMeshPro winningText;
    public TextMeshPro highestScoreText;
    public int highestScore;

    public AudioSource backGroundAudio;
    public AudioSource deathAudio;
    public AudioSource gameOverAudio;
    public AudioSource winningAudio;

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }

    private void Start()
    {
        NewGame();
        LoadFromJson();
    }

    private void Update()
    {
        if (lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        winningAudio.Stop();
        backGroundAudio.Play();
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        gameOverText.enabled = false;
        winningText.enabled = false;

        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
        gameOverText.enabled = true;
        gameOverAudio.Play();
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].gameObject.SetActive(false);
        }
        SaveToJson();
        pacman.gameObject.SetActive(false);
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = "Lives : x" + lives.ToString();
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = "Score :" + score.ToString().PadLeft(2, '0');
        if (score >= highestScore)
        {
            highestScore = score;
            highestScoreText.text = "Top Score: " + highestScore.ToString();
        }
    }

    public void PacmanEaten()
    {
        pacman.DeathSequence();
        deathAudio.Play();
        SetLives(lives - 1);

        if (lives > 0)
        {
            Invoke(nameof(ResetState), 3f);
        }
        else
        {
            GameOver();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;
        SetScore(score + points);

        ghostMultiplier++;
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        SetScore(score + pellet.points);

        if (!HasRemainingPellets())
        {
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewGame), 3f);
            winningText.enabled = true;
            backGroundAudio.Stop();
            winningAudio.Play();
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        SaveToJson();
        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
    public void SaveToJson ()
    {
        string highestScoreData = JsonUtility.ToJson(highestScore);
        string filePath = Application.persistentDataPath + "/HighestScore.json";
        System.IO.File.WriteAllText(filePath, highestScoreData);
    }
    public void LoadFromJson ()
    {
        string filePath = Application.persistentDataPath + "/HighestScore.json";
        string highestScoreData = System.IO.File.ReadAllText(filePath);
        highestScore = JsonUtility.FromJson<int>(highestScoreData);
    }
}
