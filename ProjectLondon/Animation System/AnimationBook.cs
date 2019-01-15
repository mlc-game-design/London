using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectLondon
{
    public class AnimationBook
    {
        public string Name { get; protected set; }
        public Dictionary<string,Animation> Animations { get; protected set; }

        public AnimationBook(string name)
        {
            Name = name;
            Animations = new Dictionary<string, Animation>();
        }
        public AnimationBook(string name, Dictionary<string,Animation> animations)
        {
            Name = name;
            Animations = animations.ToDictionary(kv => kv.Key, kv => kv.Value.Clone() as Animation);
        }
        public AnimationBook(AnimationBook AnimationBook)
        {
            Name = AnimationBook.Name;
            Animations = AnimationBook.Animations.ToDictionary(kv => kv.Key, kv => kv.Value.Clone() as Animation);
        }

        public void AddAnimation(string referenceName, Animation animation)
        {
            if(Animations.Count() == 0)
            {
                Animations.Add(referenceName, animation);
                return;
            }
            else
            {
                if(Animations.ContainsKey(referenceName) == false)
                {
                    Animations.Add(referenceName, animation);
                    return;
                }
                else
                {
                    Animations[referenceName] = animation;
                    return;
                }
            }
        }
        public void RemoveAnimation(string referenceName)
        {
            if (Animations.ContainsKey(referenceName))
            {
                Animations.Remove(referenceName);
                return;
            }
        }

        public Animation GetAnimation(string referenceName)
        {
            Animation _animation = null;

            if(Animations.ContainsKey(referenceName) == true)
            {
                _animation = Animations[referenceName];
            }

            return _animation;
        }
    }
}
