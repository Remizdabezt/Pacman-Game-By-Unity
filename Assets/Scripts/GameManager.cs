using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    public SaveData saveData = new();
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public TextMeshPro gameOverText;
    public TextMeshPro scoreText;
    public TextMeshPro livesText;
    public TextMeshPro winningText;
    public TextMeshPro highestScoreText;

    public AudioSource backGroundAudio;
    public AudioSource deathAudio;
    public AudioSource gameOverAudio;
    public AudioSource winningAudio;
    
    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }

    private void Start()
    {
        if (MenuManager.startAction == 0)
        {
            NewGame();
        }
        else
        {
            LoadGame();
        }
    }

    private void Update()
    {
        if (lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private void NewGame()
    {
        winningAudio.Stop();
        backGroundAudio.Play();
        SetScore(0);
        SetHighScore();
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
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", this.score);
            SetHighScore();
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
            ghosts[i].frightenned.Enable(pellet.duration);
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
        
        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
    
  
    [System.Serializable]
    public class SaveData
    {
        
        public Vector2 pacmanDirection;
        public Vector3 pacmanPosition;
        public bool isPacmanActive;
        public int score;
        public int lives;
        public bool[] pellets;
        public Vector2[] ghostDirection;
        public Vector3[] ghostPosition;
        public bool[] ghostHome;
        public bool[] ghostFrightened;
        public bool[] ghostChase;
        public bool[] ghostScatter;
        public float[] ghostHomeDuration;
        public float[] ghostChaseDuration;
        public float[] ghostFrightenedDuration;
        public float[] ghostScatterDuration;
        public float[] ghostElapsed;
    }
    private void SaveGhostData(SaveData saveData, int index)
    {
        saveData.ghostDirection[index] = this.ghosts[index].movement.direction;
        saveData.ghostPosition[index] = this.ghosts[index].transform.position;
        saveData.ghostHome[index] = this.ghosts[index].home.enabled;
        saveData.ghostChase[index] = this.ghosts[index].chase.enabled;
        saveData.ghostFrightened[index] = this.ghosts[index].frightenned.enabled;
        saveData.ghostScatter[index] = this.ghosts[index].scatter.enabled;
        saveData.ghostHomeDuration[index] = this.ghosts[index].home.DurationRemaining();
        saveData.ghostChaseDuration[index] = this.ghosts[index].chase.DurationRemaining();
        saveData.ghostFrightenedDuration[index] = this.ghosts[index].frightenned.DurationRemaining();
        saveData.ghostScatterDuration[index] = this.ghosts[index].scatter.DurationRemaining();
        saveData.ghostElapsed[index] = this.ghosts[index].home.elapsed;
    }
    private void LoadGhostData(SaveData loadData, int index)
    {
        this.ghosts[index].SetPosition(loadData.ghostPosition[index]);
        this.ghosts[index].movement.isLoad = true;
        this.ghosts[index].movement.SetDirection(loadData.ghostDirection[index], true);
        if (loadData.ghostHome[index])
            this.ghosts[index].home.Enable(loadData.ghostHomeDuration[index]);
        else
            this.ghosts[index].home.Disable();

        if (loadData.ghostChase[index])
            this.ghosts[index].chase.Enable(loadData.ghostChaseDuration[index]);
        else
            this.ghosts[index].chase.Disable();

        if (loadData.ghostFrightened[index])
            this.ghosts[index].frightenned.Enable(loadData.ghostFrightenedDuration[index]);
        else
            this.ghosts[index].frightenned.Disable();

        if (loadData.ghostScatter[index])
            this.ghosts[index].scatter.Enable(loadData.ghostScatterDuration[index]);
        else
            this.ghosts[index].scatter.Disable();

        if (loadData.ghostElapsed[index] != 0)
            this.ghosts[index].home.StartExit(loadData.ghostElapsed[index]);
    }
    private void LoadGame()
    {
        int index = 0;
        string filePath = Application.persistentDataPath + "/SaveGame.json";
        string json = System.IO.File.ReadAllText(filePath);
        SaveData loadData = JsonUtility.FromJson<SaveData>(json);
        if (loadData.isPacmanActive)
        {
            this.pacman.SetPosition(loadData.pacmanPosition);
            this.pacman.movement.SetDirection(loadData.pacmanDirection, true);
        }
        else
        {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(ResetState), 3f);
        }
        this.pacman.movement.isLoad = true;
        this.SetScore(loadData.score);
        this.SetLives(loadData.lives);
        foreach (Transform pellet in this.pellets)
        {
            if (loadData.pellets[index] == true)
            {
                pellet.gameObject.SetActive(true);
            }
            else
            {
                pellet.gameObject.SetActive(false);
            }
            index++;
        }
        for (int i = 0; i < ghosts.Length; i++)
        {
            LoadGhostData(loadData, i);
        }
        SetHighScore();
        backGroundAudio.Play();
    }

    public void SaveGame()
    {
        SaveData saveData = new();
        saveData.pacmanDirection = this.pacman.movement.direction;
        saveData.pacmanPosition = this.pacman.transform.position;
        saveData.isPacmanActive = this.pacman.gameObject.activeSelf;
        saveData.score = this.score;
        saveData.lives = this.lives;
        saveData.pellets = new bool[this.pellets.childCount];
        saveData.ghostPosition = new Vector3[this.ghosts.Length];
        saveData.ghostDirection = new Vector2[this.ghosts.Length];
        saveData.ghostHome = new bool[this.ghosts.Length];
        saveData.ghostFrightened = new bool[this.ghosts.Length];
        saveData.ghostChase = new bool[this.ghosts.Length];
        saveData.ghostScatter = new bool[this.ghosts.Length];
        saveData.ghostHomeDuration = new float[this.ghosts.Length];
        saveData.ghostChaseDuration = new float[this.ghosts.Length];
        saveData.ghostFrightenedDuration = new float[this.ghosts.Length];
        saveData.ghostScatterDuration = new float[this.ghosts.Length];
        saveData.ghostElapsed = new float[this.ghosts.Length];

        int index = 0;
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                saveData.pellets[index] = true;
            }
            else
            {
                saveData.pellets[index] = false;
            }
            index++;
        }
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            SaveGhostData(saveData, i);
        }

        string json = JsonUtility.ToJson(saveData);
        string filePath = Application.persistentDataPath + "/SaveGame.json";
        System.IO.File.WriteAllText(filePath, json);
    }
    private void SetHighScore()
    {
        highestScoreText.text = "Top Score: " + PlayerPrefs.GetInt("HighScore").ToString();
    }
}


