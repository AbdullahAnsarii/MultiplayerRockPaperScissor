using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public event Action<Selections> EnemySelected;

    private Animator animator;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        spriteRenderer = this.transform.GetChild(0).GetComponent<SpriteRenderer>();

        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, Camera.main.nearClipPlane));

        transform.position = new Vector3(point.x, point.y, 5);

        gameManager.StartGame += ReadyPlayer;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ReadyPlayer()
    {
        animator.SetTrigger("Shake");

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        animator.SetTrigger("Shake");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); //+animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        ShakeEnded();
    }

    private void ShakeEnded()
    {
        Selections selection;

        if(gameManager.EnemySelection != null)
        {
            selection = (Selections)gameManager.EnemySelection;
        }
        else
        {
            selection = (Selections)UnityEngine.Random.Range(0, 3);
        }

        spriteRenderer.sprite = Sprites.Inctance.RockPaperScissors[(int)selection];
        EnemySelected?.Invoke(selection);
    }
}
