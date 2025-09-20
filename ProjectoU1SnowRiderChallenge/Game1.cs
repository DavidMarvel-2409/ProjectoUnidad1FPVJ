using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectoU1SnowRiderChallenge
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _tuerca;
        private Texture2D _texture_bace;
        private Texture2D _pelota;
        private int al;
        private bool GameOver = false;

        private Rectangle Meta;

        Var V1, V2, V3;

        Player P1;

        private int ancho_global;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            al = 900;
            int an = (al * 16) / 9;
            _graphics.PreferredBackBufferWidth = an;
            _graphics.PreferredBackBufferHeight = al;
            _graphics.ApplyChanges();
            ancho_global = GraphicsDevice.Viewport.Width / 40;
            V1 = new Var(new Vector2(((200f * al) / 500), ((63f * al) / 500)), new Vector2(((542f * al) / 500), ((148f * al) / 500)), ancho_global, 0.01f, 0.7f, true);
            V2 = new Var(new Vector2(((169f * al) / 500), ((262f * al) / 500)), new Vector2(((697f * al) / 500), ((182f * al) / 500)), ancho_global, 0.1f, 0.5f, true);
            V3 = new Var(new Vector2(((47f * al) / 500), ((313f * al) / 500)), new Vector2(((463f * al) / 500), ((425f * al) / 500)), ancho_global, 0.5f, 0.8f, true);

            Meta = new Rectangle((int)ancho_global * 22, (int)ancho_global * 20, ancho_global * 5, ancho_global * 3);

            Vector2 posP = new Vector2(((225 * al) / 500), ((4 * al) / 225));
            P1 = new Player(posP, (int)(ancho_global * 1.7F), 3, Meta);

            V1.move();
            V2.move();
            V3.move();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Fuente");
            _tuerca = Content.Load<Texture2D>("Tuerca");
            _pelota = Content.Load<Texture2D>("Pelota");
            _texture_bace = new Texture2D(GraphicsDevice, 1, 1);
            _texture_bace.SetData(new[] { Color.White });

        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouse = Mouse.GetState();
            float scale_ = (float)_graphics.PreferredBackBufferHeight / 500f;

            if (!GameOver)
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (!P1.is_started)
                    {
                        V1.move();
                        V2.move();
                        V3.move();
                        P1.move_mouse();
                    }
                    if (Meta.Contains(mouse.Position))
                    {
                        Meta.X = mouse.X - Meta.Width / 2;
                        Meta.Y = mouse.Y - Meta.Height / 2;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.R))
                    P1.Re_Spawn();

                if (Keyboard.GetState().IsKeyDown(Keys.Space) && !P1.is_jump)
                {
                    P1.Jump(gameTime,scale_);
                    P1.is_jump = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.Space))
                {
                    P1.is_jump = false;
                }


                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    P1.is_started = true;
                    P1.Origen = P1.Position;
                }

                if (P1.is_started)
                    P1.update_(gameTime, V1, V2, V3, scale_);
            }

            if (P1.is_win || P1.vidas == 0)
                GameOver = true;

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            float scale_t = (float)_graphics.PreferredBackBufferHeight / 500f;

            V1.dd(_spriteBatch, _tuerca, _texture_bace, _font, scale_t);
            V2.dd(_spriteBatch, _tuerca, _texture_bace, _font, scale_t);
            V3.dd(_spriteBatch, _tuerca, _texture_bace, _font, scale_t);

            P1.dr(_spriteBatch, _pelota, _texture_bace, _font, scale_t);

            _spriteBatch.Draw(_texture_bace, Meta, Color.Green);
            Vector2 tM = new Vector2(_font.MeasureString("META").X / 2 + Meta.X, _font.MeasureString("META").Y / 2 + Meta.Y);
            _spriteBatch.DrawString(_font, "META", tM, Color.DarkGreen, 0f, Vector2.Zero, scale_t, SpriteEffects.None, 1f);

            if (GameOver)
            {
                Rectangle bo = new Rectangle(ancho_global, ancho_global, GraphicsDevice.Viewport.Width - (ancho_global * 2), GraphicsDevice.Viewport.Height - (ancho_global * 2));
                Rectangle _bo = new Rectangle(bo.X - (ancho_global / 2), bo.Y - (ancho_global / 2), bo.Width + ancho_global, bo.Height + ancho_global);
                Color _bo_color = new Color(27f / 255f, 42f / 255f, 92f / 255f);
                _spriteBatch.Draw(_texture_bace, _bo, _bo_color);
                if (P1.vidas != 0)
                {
                    Color bo_color = new Color(74f / 255f, 200f / 255f, 152f / 255f);
                    Color Text_color = new Color(44f / 255f, 68f / 255f, 196f / 255f);
                    string text = "GANASTE!!\nFelicidades, ganaste con:\n" + P1.vidas + " vidas\nTiempo: " + (Math.Round(P1.time, 3) + "s");
                    Vector2 pos_text = new Vector2(bo.Center.X, bo.Center.Y) - (_font.MeasureString(text) / 2);
                    _spriteBatch.Draw(_texture_bace, bo, bo_color);
                    _spriteBatch.DrawString(_font, text, pos_text, Text_color, 0f, Vector2.Zero, scale_t, SpriteEffects.None, 1f);
                }
                else
                {
                    Color bo_color = new Color(146f / 255f, 78f / 255f, 109f / 255f);
                    _spriteBatch.Draw(_texture_bace, bo, bo_color);
                    Color Text_color = new Color(64f / 255f, 9f / 255f, 9f / 255f);
                    string text = "GameOver\nEs una pena :(\nTiempo: " + (Math.Round(P1.time, 3) + "s");
                    Vector2 pos_text = new Vector2(bo.Center.X, bo.Center.Y) - (_font.MeasureString(text) / 2);
                    _spriteBatch.DrawString(_font, text, pos_text, Text_color, 0f, Vector2.Zero, scale_t, SpriteEffects.None, 1f);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        public static bool PointNearLine(Vector2 p, Vector2 a, Vector2 b, float tolerance)
        {
            // Vector de la línea
            Vector2 ab = b - a;
            Vector2 ap = p - a;

            float t = Vector2.Dot(ap, ab) / ab.LengthSquared();
            t = MathHelper.Clamp(t, 0f, 1f);

            Vector2 closest = a + t * ab;
            float dist = Vector2.Distance(p, closest);

            return dist <= tolerance;
        }


        public class Player
        {
            public Rectangle Box, BoxInfo, mmeta;
            public Vector2 Position, Origen, normal;
            public float speed, angle, acc, time, peso;
            public int vidas;
            public bool is_started, is_jump, air, is_win = false;
            public Player(Vector2 po, int an, int vids, Rectangle Meta)
            {
                is_started = false;
                Position = po;
                vidas = vids;
                speed = 0;
                angle = MathHelper.PiOver2;
                acc = 0;
                time = 0;
                peso = 0;
                Box = new Rectangle((int)Position.X, (int)Position.Y, an, an);
                normal = new Vector2();
                is_jump = false;
                air = true;
                BoxInfo = new Rectangle(an / 2, an / 2, an * 5, (int)(an * 1.5f));
                mmeta = Meta;
            }
            public void Re_Spawn()
            {
                Position = Origen;
                angle = MathHelper.PiOver2;
                speed = 0;
            }

            public void dr(SpriteBatch _sb, Texture2D _pelota, Texture2D _base, SpriteFont _fue, float scale)
            {
                _sb.Draw(_pelota, Box, Color.White);

                Color fondo = new Color(29f / 255f, 73f / 255f, 88f / 255f);
                Color TEXT = new Color(188f / 255f, 229f / 255, 217f / 225f);
                _sb.Draw(_base, BoxInfo, fondo);
                _sb.DrawString(_fue, "S: X" + (int)Position.X + ".Y" + (int)Position.Y + "m " + "V: " + (int)speed + "m/s" + "\nVidas: " + vidas + " T: " + (Math.Round(time, 1)) + "s", new Vector2(BoxInfo.X + 10, BoxInfo.Y + 10), TEXT, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

            }

            public void move_mouse()
            {
                var mouse = Mouse.GetState();

                if (Box.Contains(mouse.Position))
                {
                    Box.X = mouse.Position.X - Box.Width / 2;
                    Box.Y = mouse.Position.Y - Box.Height / 2;
                    Position.X = Box.X;
                    Position.Y = Box.Y;
                }
            }

            public void Start_()
            {
                Origen = Position;
                peso = 5;
                acc = 9.8f;
                is_started = true;
            }
            public void update_(GameTime gameTime, Var V1, Var V2, Var V3, float scale)
            {
                //colision
                // detectar sobre qué rampa está
                Var rampaActiva = null;

                Vector2 centerP = new Vector2(Box.Center.X, Box.Y + Box.Height);

                if (PointNearLine(centerP, V1.BoxA.Center.ToVector2(), V1.BoxB.Center.ToVector2(), V1.Ramp_width / 2))
                    rampaActiva = V1;
                else if (PointNearLine(centerP, V2.BoxA.Center.ToVector2(), V2.BoxB.Center.ToVector2(), V2.Ramp_width / 2))
                    rampaActiva = V2;
                else if (PointNearLine(centerP, V3.BoxA.Center.ToVector2(), V3.BoxB.Center.ToVector2(), V3.Ramp_width / 2))
                    rampaActiva = V3;

                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (is_started)
                    time += (1 * dt);

                if (rampaActiva != null)
                {
                    air = false;
                    // Obtener centros de las cajas de la rampa
                    Vector2 A = new Vector2(rampaActiva.BoxA.X + rampaActiva.BoxA.Width / 2,
                                            rampaActiva.BoxA.Y + rampaActiva.BoxA.Height / 2);
                    Vector2 B = new Vector2(rampaActiva.BoxB.X + rampaActiva.BoxB.Width / 2,
                                            rampaActiva.BoxB.Y + rampaActiva.BoxB.Height / 2);

                    // Gravedad
                    float g = 9.8f;

                    // Dirección unitaria de la rampa
                    Vector2 rampDir;
                    if (B.Y < A.Y)
                        rampDir = Vector2.Normalize(A - B);
                    else
                        rampDir = Vector2.Normalize(B - A);

                    // Gravedad
                    Vector2 gravity = new Vector2(0, g);

                    // Proyección: producto punto
                    float accAlongRamp = Vector2.Dot(gravity, rampDir);

                    // Fricción (opuesta al movimiento)
                    float friction = rampaActiva.muK * g * (float)Math.Cos(rampaActiva.Ramp_angle);

                    // Aceleración total
                    acc = accAlongRamp - Math.Sign(speed) * friction;

                    // Guardar ángulo de la rampa
                    angle = (float)Math.Atan2(rampDir.Y, rampDir.X);

                    speed += acc * dt;
                    Position += rampDir * speed * dt;

                    //colision con el obstaculo
                    if (damage(rampaActiva))
                    {
                        vidas -= 1;
                        Re_Spawn();
                    }
                }
                else
                {
                    // caída libre hacia abajo
                    acc = 9.8f;

                    if (angle < MathHelper.PiOver2)
                    {
                        angle += 0.5f * scale * dt;
                    }
                    if (angle > MathHelper.PiOver2)
                    {
                        angle -= 0.5f * scale * dt;
                    }

                    speed += acc * dt;

                    Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    Position += dir * speed * dt;
                    air = true;
                }

                Box.X = (int)Position.X;
                Box.Y = (int)Position.Y;
                is_win = Is_Winner();
            }

            public void Jump(GameTime gameTime, float scale)
            {
                if (!air)
                {
                    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Vector2 ramp_dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    normal = new Vector2(-ramp_dir.Y, ramp_dir.X);
                    float jump_force = 3000*scale;
                    if (normal.Y < 0) normal = -normal;

                    Vector2 jump_dir = Vector2.Normalize(normal);
                    Vector2 Jump_ = jump_dir * jump_force;
                    Position -= Jump_ * dt;

                    angle = (float)Math.Atan2(jump_dir.Y, jump_dir.X);

                }
            }

            private bool damage(Var ramp)
            {
                if (Box.Contains(ramp.Ob.X, ramp.Ob.Y) || Box.Contains(ramp.Ob.X + ramp.Ob.Width, ramp.Ob.Y))
                    return true;
                return false;
            }

            private bool Is_Winner()
            {
                if (mmeta.Contains(Box))
                    return true;
                return false;
            }

        }

        public class Var
        {
            public Rectangle BoxA, BoxB;
            public Rectangle Ramp;
            public Rectangle Ob;
            private float ob_pos_ramp;
            public float Ramp_length, Ramp_width, Ramp_angle;
            public float muK;
            public bool is_Static;

            public Var(Vector2 origen, Vector2 origenB, float ancho, float _muk, float ob_dis, bool st)
            {
                muK = _muk;
                BoxA = new Rectangle((int)origen.X, (int)origen.Y, (int)ancho, (int)ancho);
                BoxB = new Rectangle((int)(origenB.X), (int)origenB.Y, (int)ancho, (int)ancho);
                float ramp_ancho = (ancho * 30) / 100;
                float Dx = BoxB.X - BoxA.X;
                Ramp = new Rectangle(BoxA.X + ((int)ramp_ancho), BoxA.Y + ((int)ramp_ancho), (int)(Dx + (ramp_ancho)), (int)ramp_ancho);
                Ob = new Rectangle((int)(Ramp.X + (Ramp.Width * ob_dis)), Ramp.Y, (int)(ancho * 0.5f), (int)(ancho * 0.5f));
                ob_pos_ramp = ob_dis;
                is_Static = st;
            }

            public void dd(SpriteBatch _sb, Texture2D _tuerca, Texture2D _base, SpriteFont fuente, float scale)
            {
                if (!is_Static)
                {
                    _sb.Draw(_tuerca, BoxA, Color.White);
                    _sb.Draw(_tuerca, BoxB, Color.White);
                }
                Vector2 rr = new Vector2(Ramp.X, Ramp.Y);
                _sb.Draw(_base, rr, null, Color.White, Ramp_angle, new Vector2(0, 0.5f), new Vector2(Ramp_length, Ramp_width), SpriteEffects.None, 0f);
                Vector2 obb = new Vector2(Ob.X, Ob.Y);
                _sb.Draw(_base, obb, null, Color.Brown, Ramp_angle, new Vector2(0, 0.5f), new Vector2(Ob.Width, Ob.Width), SpriteEffects.None, 0f);
                draw_info(_sb, fuente, scale);

            }

            public void move()
            {
                var mouse = Mouse.GetState();
                if (BoxA.Contains(mouse.Position))
                {
                    BoxA.X = mouse.X - BoxA.Width / 2;
                    BoxA.Y = mouse.Y - BoxA.Height / 2;
                }
                else if (BoxB.Contains(mouse.Position))
                {
                    BoxB.X = mouse.X - BoxB.Width / 2;
                    BoxB.Y = mouse.Y - BoxB.Height / 2;
                }
                update_ramp();
            }

            private void update_ramp()
            {
                // Centro de BoxA y BoxB
                Vector2 centerA = new Vector2(BoxA.X + BoxA.Width / 2, BoxA.Y + BoxA.Height / 2);
                Vector2 centerB = new Vector2(BoxB.X + BoxB.Width / 2, BoxB.Y + BoxB.Height / 2);

                // Vector entre los dos puntos
                Vector2 delta = centerB - centerA;

                // Longitud de la rampa (distancia entre los centros)
                Ramp_length = delta.Length();

                // Grosor (proporcional al tamaño de las cajas)
                Ramp_width = BoxA.Width * 0.3f;

                // Posición = punto medio
                Ramp.X = (int)centerA.X;
                Ramp.Y = (int)centerA.Y;

                // Ángulo hacia B
                Ramp_angle = (float)Math.Atan2(delta.Y, delta.X);

                //obstaculo
                Vector2 dir = Vector2.Normalize(delta);
                Vector2 ob_pos = new Vector2(0, 0);
                ob_pos = centerA + dir * (Ramp_length * ob_pos_ramp);
                Ob = new Rectangle((int)(ob_pos.X - (BoxA.Width / 2)), (int)(ob_pos.Y - (BoxA.Width / 2)), (int)(BoxA.Width * 0.5f), (int)(BoxA.Width * 0.5f));
            }

            private void draw_info(SpriteBatch _sb, SpriteFont fuente, float scale)
            {
                float grados = MathHelper.ToDegrees(Ramp_angle);
                string txt = "Angle: " + ((int)grados * -1) + "\nmuK: " + muK;
                Vector2 centerA = new Vector2(BoxA.Center.X, BoxA.Center.Y);
                Vector2 centerB = new Vector2(BoxB.Center.X, BoxB.Center.Y);
                Vector2 rampCenter = (centerA + centerB) / 2f;
                Vector2 textSize = fuente.MeasureString(txt);
                Vector2 tGr = rampCenter - textSize / 2f;
                tGr.Y += textSize.Y;
                // Convertir a grados
                _sb.DrawString(fuente, txt, tGr, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

            }
            public Vector2[] GetRampVertices()
            {
                Vector2 centerA = new Vector2(BoxA.X + BoxA.Width / 2, BoxA.Y + BoxA.Height / 2);
                Vector2 centerB = new Vector2(BoxB.X + BoxB.Width / 2, BoxB.Y + BoxB.Height / 2);

                Vector2 dir = Vector2.Normalize(centerB - centerA);
                Vector2 perp = new Vector2(-dir.Y, dir.X); // perpendicular para el grosor
                Vector2 offset = perp * (Ramp_width / 2);

                Vector2 v1 = centerA + offset;
                Vector2 v2 = centerB + offset;
                Vector2 v3 = centerB - offset;
                Vector2 v4 = centerA - offset;

                return new[] { v1, v2, v3, v4 };
            }
        }
    }
}
