using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GridFreeFriction
{
    class Character
    {

        Texture2D character;
        Rectangle characterRec;

        bool jumping = false;
        int jumpSpeed;

        private int leftBound;
        private int rightBound;

        bool isSelected = false;
        bool isBlockingTile = false;
        bool isStartingSquare = false;
        bool isEndingSquare = false;

        bool isNeededTile = false;

        private int iterationNum = -1;


        private bool isTouchingSolidObject = false;
        private bool hasWallToLeft = false;
        private bool hasWallToRight = false;
        private bool hasWallToTop = false;
        private bool hasWallToBottom = false;

        private bool useNormalGravity = true;
        private int startY;
        private int counter = 0;
        private string lastFacingDirection = "";

        private int upperBound, lowerBound;

        private bool isMovingUp = false;
        private bool isMovingDown = false;

        private bool isMovingLeft;
        private bool isMovingRight;


        private bool hasBeenDoubleChecked = false;


        private bool getPos = true;

        private bool shouldGetPos = true;
        private double health = 100;

        int velocity;
        private int originalPosition = 0;

        public Character(Texture2D c, Rectangle cr)
        {
            character = c;
            characterRec = cr;

        }

        //public Character(int textureID, Rectangle cr)
        //{
        //    character = c;
        //    characterRec = cr;

        //}

        //makes the player jump
        public void Jump(bool isPlayerColliding, int oldPlayerY)
        {
            bool isCollidingUpdating = isPlayerColliding;

            if (jumping == true)
            {

                characterRec.Y += jumpSpeed;
                if (jumpSpeed < 10)
                {
                    jumpSpeed++;

                }
                counter++;
                if (isCollidingUpdating == true && counter > 30)
                {
                    //characterRec.Y = startY;
                    jumping = false;
                    counter = 0;
                    useNormalGravity = true;
                }
                else if (isCollidingUpdating == true && counter > 10)
                {
                    jumping = false;
                    counter = 0;
                    useNormalGravity = true;
                }
                if (hasWallToTop == true)
                {
                    characterRec.Y = oldPlayerY;
                    jumpSpeed = 0;
                }

            }

            else
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.W) && jumping == false && hasWallToTop == false && hasWallToBottom == true))
                {
                    jumping = true;
                    startY = characterRec.Y;
                    jumpSpeed = -15;
                    //isCollidingUpdating = false;
                    useNormalGravity = false;
                }


            }

        }

        public void gravity()
        {

            if (hasWallToBottom != true)
            {
                characterRec.Y += velocity;

                if (velocity < 8)
                {
                    velocity++;
                }
            }
            else
            {

                velocity = 0;
            }


        }

        public void moveLeft(int speed)
        {

            characterRec.X -= speed;
            lastFacingDirection = "Left";

        }
        public void moveRight(int speed)
        {

            characterRec.X += speed;
            lastFacingDirection = "Right";

        }
        public void moveUp(int speed)
        {

            characterRec.Y -= speed;

        }
        public void moveDown(int speed)
        {

            characterRec.Y += speed;

        }

        public void bindLeft()
        {

            if (characterRec.X < 0)
            {
                characterRec.X = 0;
            }

        }
        public void bindTop()
        {

            if (characterRec.Y < 0)
            {
                characterRec.Y = 0;
            }

        }
        public void bindBottom(int screenHeight)
        {

            if (characterRec.Y > screenHeight - characterRec.Height)
            {
                characterRec.Y = screenHeight - characterRec.Height;
            }

        }
        public void bindRight(int screenWidth)
        {

            if (characterRec.X > screenWidth - characterRec.Width)
            {
                characterRec.X = screenWidth - characterRec.Width;
            }

        }


        //Patrol behavior
        public void patrol(int leftBoundNum, int rightBoundNum, int speed)
        {
            leftBound = leftBoundNum;
            rightBound = rightBoundNum;


            //when the boolean is true, the position will be stored
            if (getPos == true)
            {
                originalPosition = characterRec.X;
                isMovingRight = true;
                getPos = false;
            }
            //Moves the enemy right
            if (isMovingRight == true)
            {
                characterRec.X += speed;
            }
            //Stops the enemy moving right when it has moved the desired distance
            if (characterRec.X <= leftBound)
            {
                isMovingRight = true;
                isMovingLeft = false;
            }
            //Moves the enemy left
            if (isMovingLeft == true)
            {
                characterRec.X -= speed;
            }
            //Tells the enemy to stop moving left
            if (characterRec.X >= rightBound)
            {
                isMovingLeft = true;
                isMovingRight = false;
            }

        }


        //Patrol behavior
        public void moveBetween(int upperBoundNum, int lowerBoundNum, int speed)
        {
            upperBound = upperBoundNum;
            lowerBound = lowerBoundNum;


            //when the boolean is true, the position will be stored
            if (shouldGetPos == true)
            {
                originalPosition = characterRec.X;
                isMovingDown = true;
                shouldGetPos = false;
            }
            //Moves the enemy right
            if (isMovingDown == true)
            {
                characterRec.Y += speed;
            }
            //Stops the enemy moving down when it has moved the desired distance
            if (characterRec.Y <= upperBound)
            {
                isMovingDown = true;
                isMovingUp = false;
            }
            //Moves the enemy up
            if (isMovingUp == true)
            {
                characterRec.Y -= speed;
            }
            //Tells the enemy to stop moving up
            if (characterRec.Y >= lowerBound)
            {
                isMovingUp = true;
                isMovingDown = false;
            }

        }


        public int GetRektX
        {
            get
            {
                return characterRec.X;
            }
        }

        public int GetRektXLength
        {
            get
            {
                return (characterRec.Right - characterRec.Left);
            }
        }

        public int GetRektYLength
        {
            get
            {
                return (characterRec.Bottom - characterRec.Top);
            }
        }

        public int GetRektY
        {
            get
            {
                return characterRec.Y;
            }
        }

        public Rectangle GetRekt
        {
            get
            {
                return characterRec;
            }
        }

        public void setX(int position)
        {

            characterRec.X = position;

        }

        public void setY(int position)
        {

            characterRec.Y = position;

        }

        public void setWidth(int newWidth)
        {

            characterRec.Width = newWidth;

        }

        public void setHeight(int newHeight)
        {

            characterRec.Height = newHeight;

        }

        public void growCharacter(int horizontalAmount, int verticalAmount)
        {
            this.characterRec.Inflate(horizontalAmount, verticalAmount);
        }

        public void CheckJustCollision(Rectangle solidObjectRec, Character Player)
        {
            if (solidObjectRec.Intersects(Player.GetRekt))
            {
                isTouchingSolidObject = true;

            }

        }

        public void FinalCollisionCheck(Rectangle solidObjectRec, Character Player)
        {
            if (solidObjectRec.Intersects(Player.GetRekt))
            {
                isTouchingSolidObject = true;

            }
            else
            {
                isTouchingSolidObject = true;
            }


        }
        public void SideCollide(Rectangle itemRec, bool MovingRight, bool MovingLeft)
        {
            if (characterRec.Intersects(itemRec) && MovingRight == true)
            {
                characterRec.X -= 4;
            }
            if (characterRec.Intersects(itemRec) && MovingLeft == true)
            {
                characterRec.X += 4;
            }
        }
        public void CheckCollision(Rectangle solidObjectRec, Character Player)
        {
            if (solidObjectRec.Intersects(Player.GetRekt))
            {
                isTouchingSolidObject = true;

            }
            else
            {
                isTouchingSolidObject = false;
            }


            if ((Player.GetRekt.Top) <= (solidObjectRec.Bottom) && (Player.GetRekt.Bottom) >= (solidObjectRec.Top) + 2 &&
                (Player.GetRekt.Right) + 3 >= ((solidObjectRec.Left) - 1) && isTouchingSolidObject == true)
            {
                hasWallToRight = true;

            }
            else
            {
                hasWallToRight = false;

            }

            if ((Player.GetRekt.Top) <= (solidObjectRec.Bottom) && (Player.GetRekt.Bottom) >= (solidObjectRec.Top) + 2 &&
                (Player.GetRekt.Left) <= (solidObjectRec.Right + 1) && (Player.GetRekt.Right) > (solidObjectRec.Right) && isTouchingSolidObject == true)
            {
                hasWallToLeft = true;

            }
            else
            {
                hasWallToLeft = false;

            }



            if (((solidObjectRec.Top) - 1 <= (Player.GetRekt.Bottom)) && ((solidObjectRec.Bottom) > (Player.GetRekt.Bottom)) && ((Player.GetRekt.Left < solidObjectRec.Right) &&
                (Player.GetRekt.Right > solidObjectRec.Left)))
            {
                hasWallToBottom = true;
                //isColliding = true;
            }
            else
            {
                hasWallToBottom = false;
                //isColliding = false;
            }

            if (((solidObjectRec.Bottom) + 1 > (Player.GetRekt.Top)) && ((solidObjectRec.Bottom) < (Player.GetRekt.Bottom)) && ((Player.GetRekt.Left < solidObjectRec.Right) &&
                (Player.GetRekt.Right > solidObjectRec.Left)))
            {
                hasWallToTop = true;

            }
            else
            {
                hasWallToTop = false;

            }

        }

        //public int SetRektX
        //{
        //    set
        //    {

        //        x = characterRec.X;
        //    }

        //}

        public bool getIsTouchingSolidObject
        {
            get
            {
                return isTouchingSolidObject;
            }
        }

        public bool getHasWallToTop
        {
            get
            {
                return hasWallToTop;
            }
        }

        public bool getHasWallToBottom
        {
            get
            {
                return hasWallToBottom;
            }
        }

        public bool getHasWallToLeft
        {
            get
            {
                return hasWallToLeft;
            }
        }

        public bool getHasWallToRight
        {
            get
            {
                return hasWallToRight;
            }
        }

        public bool getIsJumping
        {
            get
            {
                return jumping;
            }
        }

        public bool getUseNormalgravity
        {
            get
            {
                return useNormalGravity;
            }
        }

        public int getCounter
        {
            get
            {
                return counter;
            }
        }

        public double getHealth
        {
            get
            {
                return health;
            }
        }


        public void setHealth(double newHealth)
        {
            this.health = newHealth;

        }

        public void setIsBlockingTile(bool newValue)
        {
            isBlockingTile = newValue;

        }

        public void setIterationNum(int newValue)
        {
            iterationNum = newValue;

        }

        public void setIsStartingSquare(bool newValue)
        {
            isStartingSquare = newValue;

        }

        public void setIsEndingSquare(bool newValue)
        {
            isEndingSquare = newValue;

        }

        public void setIsNeededTile(bool newValue)
        {
            isNeededTile = newValue;

        }

        public void setHasBeenDoubleChecked(bool newValue)
        {
            hasBeenDoubleChecked = newValue;

        }

        //public void setIsSelected(bool newValue)
        //{
        //    isKeyTile = isSelected;

        //}

        public bool getIsNeededTile()
        {
            return isNeededTile;
        }

        public bool getIsSelected()
        {
            return isSelected;
        }

        public bool getIsBlockingTile()
        {
            return isBlockingTile;
        }

        public bool getIsStartingSquare()
        {
            return isStartingSquare;
        }

        public bool getIsEndingSquare()
        {
            return isEndingSquare;
        }

        public bool getHasbeenDoubleChecked()
        {
            return hasBeenDoubleChecked;
        }

        public int getIterationNum()
        {
            return iterationNum;
        }


        public string getLastFacingDirect
        {
            get
            {
                return lastFacingDirection;

            }
        }
        public void changeImage(Texture2D CurrentPic)
        {
            character = CurrentPic;
        }

        public void DrawCharacter(SpriteBatch sb)
        {

            sb.Draw(character, characterRec, Color.White);

        }

        public void DrawCharacter(SpriteBatch sb, Color color)
        {

            sb.Draw(character, characterRec, color);

        }
    }
}

