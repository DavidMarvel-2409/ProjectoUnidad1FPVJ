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

        Var V1, V2, V3;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Fuente");
            _tuerca = Content.Load<Texture2D>("Tuerca");
            _texture_bace = new Texture2D(GraphicsDevice, 1, 1);
            _texture_bace.SetData(new[] { Color.White });

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                V1.move();
                V2.move();
                V3.move();

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            V1.dd(_spriteBatch, _tuerca, _texture_bace, _font);
            V2.dd(_spriteBatch, _tuerca, _texture_bace, _font);
            V3.dd(_spriteBatch, _tuerca, _texture_bace, _font);

            _spriteBatch.End();

            base.Draw(gameTime);
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

            public void dd(SpriteBatch _sb, Texture2D _tuerca,Texture2D _base, SpriteFont fuente)
            {
                _sb.Draw(_tuerca, BoxA, Color.White);
                _sb.Draw(_tuerca, BoxB, Color.White);
                //_sb.Draw(_base, Ramp, Color.White);
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
        }
    }
}
