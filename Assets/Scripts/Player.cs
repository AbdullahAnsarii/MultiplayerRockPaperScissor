using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Animator animator;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

    private Camera mainCamera;

    private Selections selection;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        animator = this.GetComponent<Animator>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();

        Vector3 point = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, mainCamera.nearClipPlane));

        transform.position = new Vector3(point.x, point.y, 5);
        
        gameManager.PlayerSelected += ReadyPlayer;
        gameManager.StartGame += GameManager_StartGame;
    }

    private void GameManager_StartGame()
    {
        StartCoroutine(StartGame());
    }

    private void ReadyPlayer(Selections selection)
    {
        this.selection = selection;
        //animator.SetTrigger("Shake");
    }

    private IEnumerator StartGame()
    {
        animator.SetTrigger("Shake");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); //+animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        ShakeEnded();
    }

    private void ShakeEnded()
    {
        spriteRenderer.sprite = Sprites.Inctance.RockPaperScissors[(int)selection];
    }
}
