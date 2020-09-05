using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprites : MonoBehaviour
{
    public static Sprites Inctance { get; set; }

    public List<Sprite> RockPaperScissors;

    // Start is called before the first frame update
    void Start()
    {
        Inctance = this;
    }
}
