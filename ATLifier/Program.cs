using System;
using System.Collections.Generic;

namespace ATLifier
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> spritedef = new List<string>();

            Console.WriteLine("Please enter the spritecode for the sprite you want to generate: ");
            string spritecode = Console.ReadLine();
            Console.Clear();

            if (new List<char>{'h','d'}.Contains(spritecode[1]))
            {
                //We're generating a sprite which doesn't need an atl vers
                spritedef = BuildOldSprite(GetSpriteFromCode(spritecode));
                spritedef.Add("");
                spritedef.Add(BuildSpriteAlias(spritecode));
            }

            else if (new List<char>{'k','n'}.Contains(spritecode[1]))
            {
                //We're generating a wink sprite
                List<string> oldspritedef = BuildOldSprite(GetSpriteFromCode(spritecode));
                oldspritedef[0] = oldspritedef[0].Replace("_static","");
                spritedef = BuildWinkSprite(GetSpriteFromCode(spritecode));
                spritedef.Add("");
                spritedef.AddRange(BuildOldSprite(GetSpriteFromCode(spritecode)));

                //Now we get the normal eyes version
                string normaleyes = spritecode[0].ToString() + "e" + spritecode.Substring(2,spritecode.Length-2);
                spritedef.Add("");
                spritedef.AddRange(BuildOldSprite(GetSpriteFromCode(normaleyes)));
            }

            else
            {
                //We are generating a sprite which needs atl
                List<string> oldspritedef = BuildOldSprite(GetSpriteFromCode(spritecode));
                oldspritedef[0] = oldspritedef[0].Replace("_static","");
                spritedef = CreateNewSprite(oldspritedef);
                spritedef.Add("");
                spritedef.AddRange(BuildOldSprite(GetSpriteFromCode(spritecode)));

                //Now we get the eyes closed version
                string closedeyes = spritecode[0].ToString() + "d" + spritecode.Substring(2,spritecode.Length-2);
                spritedef.Add("");
                spritedef.AddRange(BuildOldSprite(GetSpriteFromCode(closedeyes)));
                spritedef.Add("");
                spritedef.Add(BuildSpriteAlias(closedeyes));
            }

            foreach (string line in spritedef)
            {
                Console.WriteLine(line);
            }

            //Keep VSC open
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }

        public static string BuildSpriteAlias(string spritecode)
        {
            return $"image monika {spritecode} = \"monika {spritecode + "_static"}\"";
        }

        public static List<string> AdjustSpriteName(List<string> old_sprite_def)
        {
            old_sprite_def[0] = old_sprite_def[0].Replace(GetSpriteCode(old_sprite_def[0]),GetSpriteCode(old_sprite_def[0]) + "_static");
            return old_sprite_def;
        }

        public static List<string> CreateNewSprite(List<string> old_sprite_def)
        {
            //So we don't do extra work
            foreach (string s in old_sprite_def)
            {
                if (s.Contains("closedsad") || s.Contains("closedhappy") || s.Contains("wink"))
                    return new List<string>();
            }

            List<string> new_sprite_def = new List<string>();
            MonikaSprite spr = GetSpriteFromCode(GetSpriteCode(old_sprite_def[0]));
            spr.eyes = "closedsad";

            new_sprite_def.Add($"image monika {GetSpriteCode(old_sprite_def[0])}:");
            new_sprite_def.Add("    block:");
            new_sprite_def.Add($"        \"monika {GetSpriteCode(old_sprite_def[0])}_static\"");
            new_sprite_def.Add("        block:");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                3");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                5");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                7");
            new_sprite_def.Add($"        \"monika {GetSpriteCodeFromObject(spr)}_static\"");
            new_sprite_def.Add("        0.05");
            new_sprite_def.Add("        repeat");
            new_sprite_def.Add("");
            return new_sprite_def;
        }

        public static List<string> BuildOldSprite(MonikaSprite spr)
        {
            List<string> sprite_def = new List<string>();

            sprite_def.Add($"image monika {GetSpriteCodeFromObject(spr)}_static = DynamicDisplayable(");
            sprite_def.Add($"    {spr.method},");
            sprite_def.Add($"    {spr.character},");
            sprite_def.Add($"    eyes=\"{spr.eyes}\",");
            sprite_def.Add($"    eyebrows=\"{spr.eyebrows}\",");
            sprite_def.Add($"    nose=\"{spr.nose}\",");
            sprite_def.Add($"    mouth=\"{spr.mouth}\",");

            if (spr.blush != "")
                sprite_def.Add($"    blush=\"{spr.blush}\",");
            if (spr.tears != "")
                sprite_def.Add($"    tears=\"{spr.tears}\",");
            if (spr.sweat != "")
                sprite_def.Add($"    sweat=\"{spr.sweat}\",");

            sprite_def.Add($"    head=\"{spr.head}\",");
            sprite_def.Add($"    left=\"{spr.left}\",");
            sprite_def.Add($"    right=\"{spr.right}\",");
            sprite_def.Add($"    arms=\"{spr.arms}\",");

            if (spr.arms == "def")
            {
                sprite_def.Add("    lean=\"def\",");
                sprite_def.Add("    single=\"3b\"");
            }

            sprite_def.Add(")");
            sprite_def.Add("");
            return sprite_def;
        }

        public static List<string> BuildWinkSprite(MonikaSprite spr)
        {
            List<string> sprite_def = new List<string>();

            sprite_def.Add($"image monika {GetSpriteCodeFromObject(spr)}:");
            sprite_def.Add("    block:");
            sprite_def.Add($"       \"{GetSpriteCodeFromObject(spr)}_static\"");
            sprite_def.Add("        1");

            spr.eyes="normal";
            sprite_def.Add($"       \"{GetSpriteCodeFromObject(spr)}\"");
            return sprite_def;
        }

        public static string GetSpriteCode(string line)
        {
            return line.Substring(13, line.IndexOf("=") - 14);
        }

        public static MonikaSprite GetSpriteFromCode(string spritecode)
        {
            string eyes="", eyebrows="", mouth="", sweat="", blush="", tears="", arms="", left="", right="", head="";

            //Eyebrows
            switch (spritecode[2])
            {
                case 'f':
                    eyebrows = "furrowed";
                    break;
                case 'u':
                    eyebrows = "up";
                    break;
                case 'k':
                    eyebrows = "knit";
                    break;
                case 's':
                    eyebrows = "mid";
                    break;
                case 't':
                    eyebrows = "think";
                    break;
            }
            spritecode = spritecode.Remove(2,1);

            //Eyes
            switch (spritecode[1])
            {
                case 'e':
                    eyes = "normal";
                    break;
                case 'w':
                    eyes = "wide";
                    break;
                case 's':
                    eyes = "sparkle";
                    break;
                case 't':
                    eyes = "smug";
                    break;
                case 'c':
                    eyes = "crazy";
                    break;
                case 'r':
                    eyes = "right";
                    break;
                case 'l':
                    eyes = "left";
                    break;
                case 'h':
                    eyes = "closedhappy";
                    break;
                case 'd':
                    eyes = "closedsad";
                    break;
                case 'k':
                    eyes = "winkleft";
                    break;
                case 'n':
                    eyes = "winkright";
                    break;
            }
            spritecode = spritecode.Remove(1,1);

            //Mouth
            switch (spritecode[spritecode.Length-1])
            {
                case 'a':
                    mouth = "smile";
                    head = "a";
                    break;
                case 'b':
                    mouth = "big";
                    head = "b";
                    break;
                case 'c':
                    mouth = "smirk";
                    head = "h";
                    break;
                case 'd':
                    mouth = "small";
                    head = "i";
                    break;
                case 'o':
                    mouth = "gasp";
                    head = "d";
                    break;
                case 'u':
                    mouth = "smug";
                    head = "j";
                    break;
                case 'w':
                    mouth = "wide";
                    head = "b";
                    break;
                case 'x':
                    mouth = "disgust";
                    head = "f";
                    break;
                case 'p':
                    mouth = "pout";
                    head = "h";
                    break;
                case 't':
                    mouth = "triangle";
                    head = "a";
                    break;
            }
            spritecode = spritecode.Remove(spritecode.Length-1,1);

            //Arms
            switch (spritecode[0])
            {
                case '1':
                    arms = "steepling";
                    left = "1l";
                    right = "1r";
                    break;
                case '2':
                    arms = "crossed";
                    left = "1l";
                    right = "2r";
                    break;
                case '3':
                    arms = "restleftpointright";
                    left = "2l";
                    right = "1r";
                    break;
                case '4':
                    arms = "pointright";
                    left = "2l";
                    right = "2r";
                    break;
                case '5':
                    arms = "def";
                    head = "";
                    break;
                case '6':
                    arms = "down";
                    left = "1l";
                    right = "1r";
                    break;
            }

            //Sweat drop
            if (spritecode.Contains("sdl"))
            {
                sweat = "def";
                spritecode.Replace("sdl","");
            }
            else if (spritecode.Contains("sdr"))
            {
                sweat = "right";
                spritecode.Replace("sdr","");
            }

            //Blush
            if (spritecode.Contains("bf"))
            {
                blush = "full";
                spritecode.Replace("bf","");
            }
            else if (spritecode.Contains("bs"))
            {
                blush = "shade";
                spritecode.Replace("bs","");
            }
            else if (spritecode.Contains("bl"))
            {
                blush = "lines";
                spritecode.Replace("bl","");
            }

            //Tears
            if (spritecode.Contains("ts"))
            {
                tears = "streaming";
                spritecode.Replace("ts","");
            }
            else if (spritecode.Contains("td"))
            {
                tears = "dried";
                spritecode.Replace("td","");
            }
            else if (spritecode.Contains("tl"))
            {
                tears = "left";
                spritecode.Replace("tl","");
            }
            else if (spritecode.Contains("tr"))
            {
                tears = "right";
                spritecode.Replace("tr","");
            }
            else if (spritecode.Contains("tp"))
            {
                tears = "pooled";
                spritecode.Replace("tp","");
            }
            else if (spritecode.Contains("tu"))
            {
                tears = "up";
                spritecode.Replace("tu","");
            }

            return new MonikaSprite(
                eyebrows: eyebrows,
                eyes: eyes,
                mouth: mouth,
                arms: arms,
                blush: blush,
                tears: tears,
                sweat: sweat,
                head: head,
                left: left,
                right: right
            );
        }

        public static string GetSpriteCodeFromObject(MonikaSprite spr)
        {
            string arms="", eyes="", eyebrows="", tears="", sweat="", blush="", mouth="";

            //Arms
            switch (spr.arms)
            {
                case "steepling":
                    arms="1";
                    break;
                case "crossed":
                    arms="2";
                    break;
                case "restleftpointright":
                    arms="3";
                    break;
                case "pointright":
                    arms="4";
                    break;
                case "def":
                    arms="5";
                    break;
                case "down":
                    arms="6";
                    break;
            }

            //Eyes
            switch (spr.eyes)
            {
                case "normal":
                    eyes="e";
                    break;
                case "wide":
                    eyes="w";
                    break;
                case "sparkle":
                    eyes="s";
                    break;
                case "smug":
                    eyes="t";
                    break;
                case "crazy":
                    eyes="c";
                    break;
                case "right":
                    eyes="r";
                    break;
                case "left":
                    eyes="l";
                    break;
                case "closedhappy":
                    eyes="h";
                    break;
                case "closedsad":
                    eyes="d";
                    break;
                case "winkleft":
                    eyes="k";
                    break;
                case "winkright":
                    eyes="n";
                    break;
            }

            //Eyebrows
            switch (spr.eyebrows)
            {
                case "furrowed":
                    eyebrows="f";
                    break;
                case "up":
                    eyebrows="u";
                    break;
                case "knit":
                    eyebrows="k";
                    break;
                case "mid":
                    eyebrows="s";
                    break;
                case "think":
                    eyebrows="t";
                    break;
            }

            //Blush
            switch (spr.blush)
            {
                case "full":
                    blush="bf";
                    break;
                case "shade":
                    blush="bs";
                    break;
                case "lines":
                    blush="bl";
                    break;
                default:
                    blush="";
                    break;
            }

            //Tears
            switch (spr.tears)
            {
                case "streaming":
                    tears="ts";
                    break;
                case "dried":
                    tears="td";
                    break;
                case "left":
                    tears="tl";
                    break;
                case "right":
                    tears="tr";
                    break;
                case "pooled":
                    tears="tp";
                    break;
                case "up":
                    tears="tu";
                    break;
                default:
                    tears="";
                    break;
            }

            //Sweat drop
            switch (spr.sweat)
            {
                case "def":
                    sweat="sdl";
                    break;
                case "right":
                    sweat="sdr";
                    break;
                default:
                    sweat="";
                    break;
            }

            //Mouth
            switch (spr.mouth)
            {
                case "smile":
                    mouth="a";
                    break;
                case "big":
                    mouth="b";
                    break;
                case "smirk":
                    mouth="c";
                    break;
                case "small":
                    mouth="d";
                    break;
                case "gasp":
                    mouth="o";
                    break;
                case "smug":
                    mouth="u";
                    break;
                case "wide":
                    mouth="w";
                    break;
                case "disgust":
                    mouth="x";
                    break;
                case "pout":
                    mouth="p";
                    break;
                case "triangle":
                    mouth="t";
                    break;
            }

            return arms + eyes + eyebrows + blush + tears + sweat + mouth;
        }
    }

    public class MonikaSprite
    {
        public string method = "mas_drawmonika";
        public string character = "monika_chr";
        public string eyebrows;
        public string eyes;
        public string nose = "def";
        public string mouth;
        public string head = "";
        public string left = "";
        public string right = "";
        public string arms;
        public string tears = "";
        public string blush = "";
        public string sweat = "";
        public string eyebags = "";

        public MonikaSprite(string eyebrows, string eyes, string mouth, string arms, string head="", string left="", string right="", string method="mas_drawmonika",string character="monika_chr", string nose="def", string tears="", string sweat="", string blush="")
        {
            this.method = "mas_drawmonika";
            this.character = "monika_chr";
            this.eyes = eyes;
            this.eyebrows = eyebrows;
            this.nose = nose;
            this.blush = blush;
            this.sweat = sweat;
            this.tears = tears;
            this.mouth = mouth;
            this.arms = arms;

            //Head
            if (head=="")
            {
                switch (mouth)
                {
                    case "smile":
                        head = "a";
                        break;

                    case "big":
                        head = "b";
                        break;

                    case "smirk":
                        head = "h";
                        break;

                    case "small":
                        head = "i";
                        break;

                    case "gasp":
                        head = "d";
                        break;

                    case "smug":
                        head = "j";
                        break;

                    case "wide":
                        head = "b";
                        break;

                    case "disgust":
                        head = "f";
                        break;

                    case "pout":
                        head = "h";
                        break;

                    case "triangle":
                        head = "a";
                        break;
                }
            }
            else
                this.head=head;

            //Left/Right
            if (left == "" && right == "")
            {
                switch (arms)
                {
                    case "steepling":
                        left = "1l";
                        right = "1r";
                        break;

                    case "crossed":
                        left = "1l";
                        right = "2r";
                        break;

                    case "restleftpointright":
                        left = "2l";
                        right = "1r";
                        break;

                    case "pointright":
                        left = "2l";

                        right = "2r";
                        break;

                    case "def":
                        head = "";
                        left = "";
                        right = "";
                        break;

                    case "down":
                        left = "1l";
                        right = "1r";
                        break;
                }
            }
            else
            {
                this.left = left;
                this.right = right;
            }
        }
    }
}