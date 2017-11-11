using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Linq;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof(PlatformerCharacter2D))]

    public class Platformer2DUserControl : MonoBehaviour
    {
        public GameObject projectilePrefab;
        private List<GameObject> Projectiles = new List<GameObject>();
        private float projectileVelocity = 10;
        private Dictionary<string, Action> keywordActions = new Dictionary<string, Action>();
        private KeywordRecognizer keywordRecognizer;
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;

        void Start()
        {
            keywordActions.Add("jump", () =>
            {
                m_Character.Move(3.0f, false, true);
            });

            keywordActions.Add("fire", () =>
            {
                shoot();
            });

            keywordActions.Add("move", () =>
            {
                for (int i = 0; i < 4; i++)
                {
                    float h = CrossPlatformInputManager.GetAxis("Horizontal");
                    m_Character.Move(1.0f, false, false);
                }
            });

            keywordRecognizer = new KeywordRecognizer(keywordActions.Keys.ToArray());
            keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;
            keywordRecognizer.Start();
        }

        private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
        {
            Debug.Log("Keyword " + args.text);
            keywordActions[args.text].Invoke();
        }
        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }


            for (int i = 0; i < Projectiles.Count; i++)
            {
                GameObject goBullet = Projectiles[i];
                if (goBullet != null)
                {
                    goBullet.transform.Translate(new Vector3(1, 0) * Time.deltaTime * projectileVelocity);

                    Vector3 bulletScreenPos = Camera.main.WorldToScreenPoint(goBullet.transform.position);
                    if (bulletScreenPos.x >= Screen.width)
                    {
                        Destroy(goBullet);
                        Projectiles.Remove(goBullet);
                    }
                }
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
        }

        private void shoot()
        {
            GameObject bullet = (GameObject)Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectiles.Add(bullet);

        }

    }
}
