using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 50f, maxSpeed = 3, maxJum = 4, jumPow = 220f;
    public bool grounded = true,faceright=true,doublejump = false;
    public Rigidbody2D r2;
    public Animator anim;
    public int ourHeart;
    public int maxHeart = 4;
    public gamemaster gm;
    public SoundManager sm;

    // Start is called before the first frame update
    void Start()
    {
        r2 = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        ourHeart = maxHeart;
        gm = GameObject.FindGameObjectWithTag("Gamemaster").GetComponent<gamemaster>();
        sm = GameObject.FindGameObjectWithTag("Sound").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Grounded", grounded);
        anim.SetFloat("Speed", Mathf.Abs(r2.velocity.x));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                grounded = false;
                doublejump = true;
                r2.AddForce(Vector2.up * jumPow);
            }
            else
            {
                if (doublejump)
                {
                    doublejump = false;
                    r2.velocity = new Vector2(r2.velocity.x, 0);
                    r2.AddForce(Vector2.up * jumPow* 0.7f);
                }
            }
                 
        }


    }
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        r2.AddForce((Vector2.right) * speed * h);
        if (r2.velocity.x > maxSpeed)
            r2.velocity = new Vector2(maxSpeed, r2.velocity.y);
        if(r2.velocity.x < -maxSpeed)
            r2.velocity = new Vector2(-maxSpeed, r2.velocity.y);

        if(r2.velocity.y > maxJum)
            r2.velocity = new Vector2(r2.velocity.x,maxJum);
        if (r2.velocity.y < -maxJum)
            r2.velocity = new Vector2(r2.velocity.x,-maxJum);

        if (h>0 && !faceright)
        {
            Flip();
        }
        if (h < 0 && faceright)
        {
            Flip();
        }
        if (grounded)
        {
            r2.velocity = new Vector2(r2.velocity.x * 0.7f, r2.velocity.y);
        }
        if (ourHeart <= 0)
        {
            Death();
        }
    }
    public void Flip()
    {
        faceright = !faceright;
        Vector3 Scale;
        Scale = transform.localScale;
        Scale.x *= -1;
        transform.localScale = Scale;
    }
    public void Death()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (PlayerPrefs.GetInt("highscore") < gm.points)
        {
            PlayerPrefs.SetInt("highscore", gm.points);
        }
    }
    public void Damage(int damage)
    {
        ourHeart -= damage;
        gameObject.GetComponent<Animation>().Play("Redplayer");
    }
    public void knockBack(float knockpow,Vector2 knockkir)
    {
        r2.velocity = new Vector2(0, 0);
        r2.AddForce(new Vector2(knockkir.x * -100, knockkir.y * knockpow));
    }
    private void OnTriggerEnter2D (Collider2D col)
    {
        
        if (col.CompareTag("Coin"))
        {
            sm.Playsound("coinss");
            Destroy(col.gameObject);
            
            gm.points += 1;
        }
    }
}
