using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button btnPlay;
    public Image title;
    public Image background;
    public Image uArrow;
    public Image dArrow;
    public Image rArrow;
    public Image lArrow;
    public Image uAttack;
    public Image dAttack;
    public Image rAttack;
    public Image lAttack;
    public Image uFollowThrough;
    public Image dFollowThrough;
    public Image rFollowThrough;
    public Image lFollowThrough;
    public Sprite spriteUArrow;
    public Sprite spriteDArrow;
    public Sprite spriteLArrow;
    public Sprite spriteRArrow;
    public Sprite signalUArrow;
    public Sprite signalDArrow;
    public Sprite signalLArrow;
    public Sprite signalRArrow;
    public Sprite earlyUArrow;
    public Sprite earlyDArrow;
    public Sprite earlyLArrow;
    public Sprite earlyRArrow;
    public Sprite lateUArrow;
    public Sprite lateDArrow;
    public Sprite lateLArrow;
    public Sprite lateRArrow;
    public bool checkInput;
    public int randomAttack;
    public Stopwatch hitWindow;
    public KeyCode[] correctInputs;
    public Image[] uiArrows;
    public Image[] uiAttacks;
    public Image[] uiFollowThrough;
    public Image[] uiMisses;
    public Sprite[,] arrowCollection;
    public int misses;
    public Image missX1;
    public Image missX2;
    public Image missX3;
    public Sprite[,] missCollection;
    public Sprite emptyX1;
    public Sprite emptyX2;
    public Sprite emptyX3;
    public Sprite filledX1;
    public Sprite filledX2;
    public Sprite filledX3;
    public Stopwatch surviveTime;
    public int combo;
    public int totalHitCount;
    public long score;
    public int hitWindowPointReward;
    public int timeAllowed;
    public int highestCombo;
    public Text scoreNum;
    public Text comboNum;
    public Image results;
    public Text highscores;
    public Text scoreText;
    public Text survivalTimeText;
    public Text highestComboText;
    public Text totalHitCountText;
    public Button btnRetry;
    public Button btnExit;
    public List<long> highscoreList;
    public string strHighScores;
    public string[] savedScores;
    public Text txtCountdown;
    public int randomSfx;
    public Button btnInstruc;
    public Button btnBack;
    public Image instrucBackground;
    public Text txtInstruc;
    public AudioSource audioSource;
    public AudioClip correctInputSfx1;
    public AudioClip correctInputSfx2;
    public AudioClip correctInputSfx3;
    public AudioClip correctInputSfx4;
    public AudioClip missSfx;
    public AudioClip playSfx;
    public AudioClip quitSfx;
    public AudioClip resultsSfx;
    public AudioClip retrySfx;
    public AudioClip[] correctSfxCollection;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("highscores"))  {
            highscoreList = new List<long>();
            savedScores = PlayerPrefs.GetString("highscores").Split(' ');
            for (int i = 0; i < savedScores.Length; i++) {
                highscoreList.Add(long.Parse(savedScores[i]));
            }
        } else {
            highscoreList = new List<long>();
        }
        
        //initalize checklInput and stopwatch var
        checkInput = false;
        highestCombo = 0;
        randomSfx = 0;
        hitWindowPointReward = 3000;
        timeAllowed = 3000;
        score = 0;
        hitWindow = new Stopwatch();
        surviveTime = new Stopwatch();

        //variable to keep up with misses
        misses = 0;

        //disable all attacks
        uAttack.gameObject.SetActive(false);
        dAttack.gameObject.SetActive(false);
        rAttack.gameObject.SetActive(false);
        lAttack.gameObject.SetActive(false);

        uFollowThrough.gameObject.SetActive(false);
        dFollowThrough.gameObject.SetActive(false);
        rFollowThrough.gameObject.SetActive(false);
        lFollowThrough.gameObject.SetActive(false);

        btnBack.gameObject.SetActive(false);
        instrucBackground.gameObject.SetActive(false);
        txtInstruc.gameObject.SetActive(false);

        //disable results UI for now
        activateResults(false);

        //2d-array of miss sprites
        missCollection = new Sprite[,] {
        {emptyX1, filledX1},
        {emptyX2, filledX2},
        {emptyX3, filledX3} };

        //create 2d array of all arrow sprites
        arrowCollection = new Sprite[,] {
            {spriteUArrow, signalUArrow, earlyUArrow, lateUArrow},
            {spriteDArrow, signalDArrow, earlyDArrow, lateDArrow },
            {spriteRArrow, signalRArrow, earlyRArrow, lateRArrow },
            {spriteLArrow, signalLArrow, earlyLArrow, lateLArrow } };

        //array of corrosponding correct inputs for 
        correctInputs = new KeyCode[] { KeyCode.W, KeyCode.S, KeyCode.D, KeyCode.A };

        //array of visual x's
        uiMisses = new Image[] { missX1, missX2, missX3 };

        //array of visual image UI components
        uiArrows = new Image[] { uArrow, dArrow, rArrow, lArrow };

        //array of corrosponding visual attack indicators
        uiAttacks = new Image[] { dAttack, uAttack, lAttack, rAttack };

        //sfx collection
        correctSfxCollection = new AudioClip[] { correctInputSfx1, correctInputSfx2, correctInputSfx3, correctInputSfx4 };

        //array of follow through attacks
        uiFollowThrough = new Image[] { dFollowThrough, uFollowThrough, rFollowThrough, lFollowThrough};
    }

    // Update is called once per frame
    async void Update()
    {
        //only check for inputs when wanted
        if (checkInput == true) {
            //if they press the correct key to dodge
            if (Input.GetKeyDown(correctInputs[randomAttack])) {
                checkInput = false;
                randomSfx = Random.Range(0, 4);
                audioSource.PlayOneShot(correctSfxCollection[randomSfx], .5f);
                uiAttacks[randomAttack].gameObject.SetActive(false);
                uiFollowThrough[randomAttack].gameObject.SetActive(true);
                uiArrows[randomAttack].GetComponent<Image>().sprite = arrowCollection[randomAttack, 2];
                totalHitCount++;
                combo++;
                comboNum.text = combo + " x";
                if (totalHitCount % 5 == 0) {
                    if (timeAllowed > 561) {
                        hitWindowPointReward = (int)(hitWindowPointReward / 1.15);
                        timeAllowed = (int)(timeAllowed / 1.15);
                    }
                }
                score += (hitWindowPointReward - hitWindow.ElapsedMilliseconds) * (combo + (totalHitCount / 5));
                scoreNum.text = "" + score;
                
                await Task.Delay(timeAllowed / 6);
                revertToAttackPhase();

            } else if ((!Input.GetKeyDown(correctInputs[randomAttack])) && (Input.anyKeyDown)) {
                checkInput = false;
                audioSource.PlayOneShot(missSfx);
                uiAttacks[randomAttack].gameObject.SetActive(false);
                uiFollowThrough[randomAttack].gameObject.SetActive(true);
                uiArrows[randomAttack].GetComponent<Image>().sprite = arrowCollection[randomAttack, 3];
                if (combo > highestCombo) {
                    highestCombo = combo;
                }
                combo = 0;
                await Task.Delay(timeAllowed / 6);
                misses++;
                uiMisses[misses - 1].GetComponent<Image>().sprite = missCollection[misses - 1, 1];
                revertToAttackPhase();

            } else if (hitWindow.ElapsedMilliseconds > timeAllowed) {
                checkInput = false;
                audioSource.PlayOneShot(missSfx);
                uiAttacks[randomAttack].gameObject.SetActive(false);
                uiFollowThrough[randomAttack].gameObject.SetActive(true);
                uiArrows[randomAttack].GetComponent<Image>().sprite = arrowCollection[randomAttack, 3];
                if (combo > highestCombo) {
                    highestCombo = combo;
                }
                combo = 0;
                await Task.Delay(timeAllowed / 6);
                misses++;
                uiMisses[misses - 1].GetComponent<Image>().sprite = missCollection[misses - 1, 1];
                revertToAttackPhase();
            }
        }
    }

    public async void playButtonClicked() {
        //hide the play button and title screen and setup play screen with all needed assets (the 4 arrows and the background/enemy)
        //NOTE: work on possible countdown before starting game
        audioSource.PlayOneShot(playSfx);
        await Task.Delay(500);
        btnInstruc.gameObject.SetActive(false);
        btnPlay.gameObject.SetActive(false);
        
        btnPlay.GetComponentInChildren<Image>().enabled = false;
        title.enabled = false;

        //create the random attack
        randomAttack = Random.Range(0, 4);

        //delay program before starting the gameplay
        await Task.Delay(750);
        txtCountdown.text = "2";
        await Task.Delay(750);
        txtCountdown.text = "1";
        await Task.Delay(750);
        txtCountdown.text = "Go!";
        await Task.Delay(750);
        txtCountdown.gameObject.SetActive(false);

        //set visual indicators
        uiAttacks[randomAttack].gameObject.SetActive(true);
        uiArrows[randomAttack].GetComponentInChildren<Image>().sprite = arrowCollection[randomAttack, 1];

        //start timed hit window and allow inputs
        checkInput = true;
        surviveTime.Start();
        hitWindow.Start();
    }

    public void revertToAttackPhase() {
        //return to normal arrow and no attacks
        hitWindow.Stop();
        hitWindow.Reset();
        uiFollowThrough[randomAttack].gameObject.SetActive(false);
        uiArrows[randomAttack].GetComponentInChildren<Image>().sprite = arrowCollection[randomAttack, 0];
        if (misses < 3) {
            //create the random attack
            randomAttack = Random.Range(0, 4);

            //set visual indicators
            uiAttacks[randomAttack].gameObject.SetActive(true);
            uiArrows[randomAttack].GetComponentInChildren<Image>().sprite = arrowCollection[randomAttack, 1];

            //start timed hit window and allow inputs
            checkInput = true;
            hitWindow.Start();
        } else {
            //show results screen
            surviveTime.Stop();
            audioSource.PlayOneShot(resultsSfx);
            activateResults(true);
            updateHighScores();
            //show score, highest combo, and total hit count
            scoreText.text = score + "";
            highestComboText.text = highestCombo + "";
            totalHitCountText.text = totalHitCount + "";

            long milliseconds = 0;
            long seconds = 0;
            long minutes = 0;
            long hours = 0;
            //formating survival time into hours:minutes:seconds:milliseconds
            if (surviveTime.ElapsedMilliseconds < 1000) {
                //formating milliseconds
                survivalTimeText.text = surviveTime.ElapsedMilliseconds + "";
            } else if (surviveTime.ElapsedMilliseconds >= 1000) {
                //formating seconds
                seconds = surviveTime.ElapsedMilliseconds / 1000;
                milliseconds = surviveTime.ElapsedMilliseconds % (seconds * 1000);

                if (seconds >= 60) {
                    minutes = seconds / 60;
                    seconds %= (minutes * 60);

                    if (minutes >= 60) {
                        hours = minutes / 60;
                        minutes %= (hours * 60);
                    }
                }
                survivalTimeText.text = hours + ":" + minutes + ":" + seconds + ":" + milliseconds;
            } 
        }
    }

    public async void retryButtonClicked() {
        //bring game back to original start
        audioSource.PlayOneShot(retrySfx);
        surviveTime.Reset();
        highestCombo = 0;
        hitWindowPointReward = 3000;
        timeAllowed = 3000;
        score = 0;
        misses = 0;
        missX1.GetComponent<Image>().sprite = emptyX1;
        missX2.GetComponent<Image>().sprite = emptyX2;
        missX3.GetComponent<Image>().sprite = emptyX3;
        scoreNum.text = "";
        comboNum.text = "";
        activateResults(false);
        //create the random attack
        randomAttack = Random.Range(0, 4);

        //delay program before starting the gameplay
        txtCountdown.gameObject.SetActive(true);
        txtCountdown.text = "3";
        await Task.Delay(750);
        txtCountdown.text = "2";
        await Task.Delay(750);
        txtCountdown.text = "1";
        await Task.Delay(750);
        txtCountdown.text = "Go!";
        await Task.Delay(750);
        txtCountdown.gameObject.SetActive(false);

        //set visual indicators
        uiAttacks[randomAttack].gameObject.SetActive(true);
        uiArrows[randomAttack].GetComponentInChildren<Image>().sprite = arrowCollection[randomAttack, 1];

        //start timed hit window and allow inputs
        checkInput = true;
        surviveTime.Start();
        hitWindow.Start();
    }

    public void exitButtonClicked() {
        audioSource.PlayOneShot(quitSfx);
        for (int i = 0; i < highscoreList.Count; i++) {
            if (i == highscoreList.Count - 1) {
                strHighScores += highscoreList[i];
            } else {
                strHighScores += highscoreList[i] + " ";
            }
        }
        PlayerPrefs.SetString("highscores", strHighScores);
        Application.Quit();
    }

    public void backButtonClicked() {
        btnBack.gameObject.SetActive(false);
        instrucBackground.gameObject.SetActive(false);
        txtInstruc.gameObject.SetActive(false);
    }

    public void instrucButtonClicked() {
        btnBack.gameObject.SetActive(true);
        instrucBackground.gameObject.SetActive(true);
        txtInstruc.gameObject.SetActive(true);
    }

    public void updateHighScores() {
        string txtHighScores = "";
        int switchIndex = highscoreList.Count;
        if (highscoreList.Count == 0) {
            highscoreList.Add(score);
        } else {
            //add new score to list
            for (int i = highscoreList.Count - 1; i >= 0; i--) {
                if (highscoreList[i] < score) {
                    switchIndex = i;
                }
            }
            highscoreList.Insert(switchIndex, score);
            if (highscoreList.Count > 3) {
                highscoreList.RemoveAt(highscoreList.Count - 1);
            }
        }
        for (int i = 0; i < highscoreList.Count; i++) {
            txtHighScores += (i + 1) + ": " + highscoreList[i] + "\n";
        }
        highscores.text = txtHighScores;
    }

    public void activateResults(bool active) {
        results.gameObject.SetActive(active);
        highscores.gameObject.SetActive(active);
        btnRetry.gameObject.SetActive(active);
        btnExit.gameObject.SetActive(active);
        scoreText.gameObject.SetActive(active);
        survivalTimeText.gameObject.SetActive(active);
        highestComboText.gameObject.SetActive(active);
        totalHitCountText.gameObject.SetActive(active);
    }
}
