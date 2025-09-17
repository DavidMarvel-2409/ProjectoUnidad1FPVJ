using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
            int al = 500;
            int an = (al * 16) / 9;
            _graphics.PreferredBackBufferWidth = an;
            _graphics.PreferredBackBufferHeight = al;
            _graphics.ApplyChanges();
            ancho_global = GraphicsDevice.Viewport.Width / 40;
            V1 = new Var(new Vector2(ancho_global, ancho_global), ancho_global, 5, 0.7f);
            V2 = new Var(new Vector2(ancho_global, ancho_global * 3), ancho_global, 1.3f, 0.5f);
            V3 = new Var(new Vector2(ancho_global, ancho_global * 5), ancho_global, 1.4f, 0.2f);

            Meta = new Rectangle((int)ancho_global * 10, (int)ancho_global * 10, ancho_global * 5, ancho_global * 3);

            Vector2 posP = new Vector2(GraphicsDevice.Viewport.Width - (ancho_global * 3), ancho_global * 3);
            P1 = new Player(posP, (int)(ancho_global * 1.7F), 3);


            V1.BoxB.X = 200;
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

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                P1.is_started = true;
                P1.Origen = P1.Position;
            }
                

            if (P1.is_started)
                P1.update_(gameTime, V1, V2,V3);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            V1.dd(_spriteBatch, _tuerca, _texture_bace, _font, P1.is_started);
            V2.dd(_spriteBatch, _tuerca, _texture_bace, _font, P1.is_started);
            V3.dd(_spriteBatch, _tuerca, _texture_bace, _font, P1.is_started);

            P1.dr(_spriteBatch, _pelota, _texture_bace);

            _spriteBatch.Draw(_texture_bace, Meta, Color.Green);
            Vector2 tM = new Vector2(_font.MeasureString("META").X / 2 + Meta.X, _font.MeasureString("META").Y / 2 + Meta.Y);
            _spriteBatch.DrawString(_font, "META", tM, Color.DarkGreen);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public class Player
        {
            public Rectangle Box;
            public Vector2 Position, Origen;
            public float speed, angle, acc, time, peso;
            public int vidas;
            public bool is_started;
            public Player(Vector2 po, int an, int vids)
            {
                is_started = false;
                Position = po;
                vidas = vids;
                speed = 0;
                angle = 0;
                acc = 0;
                time = 0;
                peso = 0;
                Box = new Rectangle((int)Position.X, (int)Position.Y, an, an);
            }
            public void Re_Spawn()
            {
                Position = Origen;
                vidas -= 1;
                angle = 0;
                speed = 0;

            }

            public void dr(SpriteBatch _sb, Texture2D _pelota, Texture2D _base)
            {
                _sb.Draw(_pelota, Box, Color.White);
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
            public void update_(GameTime gameTime, Var V1, Var V2, Var V3)
            {
                //colision
                // detectar sobre qué rampa está
                Var rampaActiva = null;
                if (Box.Intersects(V1.GetBoundingBox()))
                {
                    rampaActiva = V1;
                    Console.WriteLine("topo");
                }
                else if (Box.Intersects(V2.GetBoundingBox()))
                {
                    rampaActiva = V2;
                    Console.WriteLine("topo");
                }
                else if (Box.Intersects(V3.GetBoundingBox()))
                {
                    rampaActiva = V3;
                    Console.WriteLine("topo");
                }

                if (rampaActiva != null)
                {
                    /*float g = 9.8f;
                    float theta = rampaActiva.Ramp_angle;
                    acc = g * (float)Math.Sin(theta) - rampaActiva.muK * g * (float)Math.Cos(theta);
                    angle = theta;*/
                    // Obtener centros de las cajas de la rampa
                    Vector2 A = new Vector2(rampaActiva.BoxA.X + rampaActiva.BoxA.Width / 2,
                                            rampaActiva.BoxA.Y + rampaActiva.BoxA.Height / 2);
                    Vector2 B = new Vector2(rampaActiva.BoxB.X + rampaActiva.BoxB.Width / 2,
                                            rampaActiva.BoxB.Y + rampaActiva.BoxB.Height / 2);

                    // Vector de la rampa
                    Vector2 rampDir = B - A;
                    rampDir.Normalize(); // Dirección unitaria

                    // Gravedad
                    float g = 9.8f;

                    // Componente de la gravedad a lo largo de la rampa
                    acc = g * rampDir.Y - rampaActiva.muK * g * rampDir.X;

                    // Guardar ángulo de la rampa para mover al jugador
                    angle = (float)Math.Atan2(rampDir.Y, rampDir.X);
                }
                else
                {
                    acc = 9.8f; // caída libre
                    angle = MathHelper.PiOver2;
                }

                //caida

                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                speed += acc * dt;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Position += dir * speed * dt;
                Box.X = (int)Position.X;
                Box.Y = (int)Position.Y;
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

            public Var(Vector2 origen, float ancho, float _muk, float ob_dis)
            {
                muK = _muk;
                BoxA = new Rectangle((int)origen.X, (int)origen.Y, (int)ancho, (int)ancho);
                BoxB = new Rectangle((int)(origen.X + ancho * 4), (int)origen.Y, (int)ancho, (int)ancho);
                float ramp_ancho = (ancho * 30) / 100;
                float Dx = BoxB.X - BoxA.X;
                Ramp = new Rectangle(BoxA.X + ((int)ramp_ancho), BoxA.Y + ((int)ramp_ancho), (int)(Dx + (ramp_ancho)), (int)ramp_ancho);
                Ob = new Rectangle((int)(Ramp.X + (Ramp.Width * ob_dis)), Ramp.Y, (int)(ancho * 0.5f), (int)(ancho * 0.5f));
                ob_pos_ramp = ob_dis;
            }

            public void dd(SpriteBatch _sb, Texture2D _tuerca,Texture2D _base, SpriteFont fuente, bool active)
            {
                if (!active)
                {
                    _sb.Draw(_tuerca, BoxA, Color.White);
                    _sb.Draw(_tuerca, BoxB, Color.White);
                }
                Vector2 rr = new Vector2(Ramp.X, Ramp.Y);
                _sb.Draw(_base, rr, null, Color.White, Ramp_angle, new Vector2(0, 0.5f), new Vector2(Ramp_length, Ramp_width), SpriteEffects.None, 0f);
                Vector2 obb = new Vector2(Ob.X, Ob.Y);
                _sb.Draw(_base, obb, null, Color.Brown, Ramp_angle, new Vector2(0, 0.5f), new Vector2(Ob.Width, Ob.Width), SpriteEffects.None, 0f);
                draw_info(_sb, fuente);
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

            private void draw_info(SpriteBatch _sb, SpriteFont fuente)
            {
                Vector2 tGr = new Vector2((int)BoxA.X - BoxA.Width, (int)BoxA.Y - BoxA.Width);
                Vector2 tMuk = new Vector2((int)BoxA.X - BoxA.Width, (int)BoxA.Y - BoxA.Width * 2);
                // Convertir a grados
                float grados = MathHelper.ToDegrees(Ramp_angle);
                _sb.DrawString(fuente, "Angle: " + ((int)grados * -1), tGr, Color.Black);
                _sb.DrawString(fuente, "muk: " + muK, tMuk, Color.Black);

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
            public Rectangle GetBoundingBox()
            {
                var verts = GetRampVertices();
                float minX = float.MaxValue, minY = float.MaxValue;
                float maxX = float.MinValue, maxY = float.MinValue;

                foreach (var v in verts)
                {
                    if (v.X < minX) minX = v.X;
                    if (v.Y < minY) minY = v.Y;
                    if (v.X > maxX) maxX = v.X;
                    if (v.Y > maxY) maxY = v.Y;
                }

                int padding = 2;

                return new Rectangle((int)minX, (int)minY - padding, (int)(maxX - minX), (int)(maxY - minY) - 2 * padding);
            }
        }
    }
}
