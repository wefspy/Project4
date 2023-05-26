using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project4
{
    public class Animate
    {
        private readonly Dictionary<Status, (Queue<Texture2D>, double)> animations;
        private (Status, Texture2D) previousAnimation;
        private double frameTimer = 0;
        private double timeLastAnimate;
        public Animate(params (Status, List<Texture2D>, double)[] animations) 
        {
            this.animations = new Dictionary<Status, (Queue<Texture2D>, double)>();
            foreach (var animation in animations)
            {
                var queque = new Queue<Texture2D>();
                foreach(var item in animation.Item2)
                    queque.Enqueue(item);
                this.animations[animation.Item1] = (queque, animation.Item3);

            }
        }

        public Texture2D GetSprite(GameTime gameTime, Status status)
        {
            frameTimer = gameTime.TotalGameTime.TotalMilliseconds;
            if (previousAnimation.Item1 != status || frameTimer - timeLastAnimate > animations[status].Item2)
            {
                timeLastAnimate = gameTime.TotalGameTime.TotalMilliseconds;
                var sprite = animations[status].Item1.Dequeue();
                animations[status].Item1.Enqueue(sprite);
                previousAnimation = (status, sprite);
                return sprite;
            }
            return previousAnimation.Item2;
        }

        //public Animate Copy()
        //{
        //    Dictionary<Status, (Queue<Texture2D>, double)> animations;
        //    foreach(var item in this.animations)
        //    {
        //        var A = item.Value.Item1.Cop
        //    }
        //}

    }
}