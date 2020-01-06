using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GridFreeFriction
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState kb, oldkb;
        MouseState mouseState, oldmouseState;
        Point mousePos;
        Random rand = new Random();
        SpriteFont customfont;

        Character[,] backgroundCharacterGrid;
        Character backgroundOutline;

        List<int> pathFindingMovementTracker = new List<int>(0);

        int gameClock = 0;
        int cooldownTimer = 0;

        int gridXDimension = 32;
        int gridYDimension = 24;

        int fallBackCounter = 0;

        int lastDirectionDoubleChecked = 1;

        int iterationNum = 1;

        int screenWidth, screenHeight;

        int startingSquareX;
        int startingSquareY;

        int endingSquareX;
        int endingSquareY;

        int solutionCheckerX = 0;
        int solutionCheckerY = 0;

        bool isMovingUp = false;
        bool isMovingDown = false;
        bool isMovingLeft = false;
        bool isMovingRight = false;

        bool hasMovedYet = false;

        bool shouldReset = false;

        int playerHighlightedX = 0;
        int playerHighlightedY = 0;

        int randomDecider = 0;

        int verticalBlockChain = 0;
        int horizontalBlockChain = 0;

        bool hasAssignedStartingValues = false;

        bool hasDoneOneTimeCode = false;

        bool isXAnimationCountingUp = true;
        bool isYAnimationCountingUp = true;

        int maxTilesToPlace = 10;

        int pathDirectionDecider;
        int blockingTilesPlaced = 0;

        //int placeBlockRandomDecider = 0;

        int testCounter = 0;
        int tilesSurroundingPathFinder = 0;

        int pathFindingX = 0;
        int pathFindingY = 0;

        int numTilesToMove = 0;

        bool isShowingWinningAnimation = false;
        //bool isAnimationMovingVertical = false;
        //bool isAnimationMovingHorizontal = true;
        int amountToAddToX = 1;
        int amountToAddToY = 0;

        int totalNeededTilesPlaced = 0;

        bool isShowingAnswer = false;

        List<int> neededBlockingTileXCoords = new List<int>(0);
        List<int> neededBlockingTileYCoords = new List<int>(0);



        int directionDecider = 0;
        int tempDecider = 0;
        bool hasChosenStartingAndEnding = false;

        int oldPathFindingDirection = 0;

        int showPuzzleCooldown = 0;

        bool hasReset = false;


        #region gamestateThings

        enum gameState
        {

            titleScreen, gamePlay, options

        }


        gameState state = gameState.gamePlay;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = 1000;
            this.graphics.PreferredBackBufferHeight = 700;

            screenWidth = this.graphics.PreferredBackBufferWidth;
            screenHeight = this.graphics.PreferredBackBufferHeight;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            customfont = Content.Load<SpriteFont>("customfont");

            backgroundCharacterGrid = new Character[gridXDimension, gridYDimension];

            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    backgroundCharacterGrid[x, y] = new Character(Content.Load<Texture2D>("buttonOutline"),
                        new Rectangle(x * (screenWidth / gridXDimension), y * (screenHeight / gridYDimension), screenWidth / gridXDimension, screenHeight / gridYDimension));


                }

            }

            backgroundOutline = new Character(Content.Load<Texture2D>("buttonOutline"), new Rectangle(0, 0, screenWidth, screenHeight));


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            kb = Keyboard.GetState();
            mouseState = Mouse.GetState();
            mousePos = new Point(mouseState.X, mouseState.Y);



            switch (state)
            {

                case gameState.titleScreen:
                    titleScreen();
                    break;
                case gameState.gamePlay:

                    gamePlay(gameTime);
                    break;
                case gameState.options:

                    options();

                    break;

            }
            //changeColor();
            oldkb = kb;
            oldmouseState = mouseState;
            base.Update(gameTime);
        }

        private void titleScreen()
        {


        }

        private void options()
        {


        }

        private void gamePlay(GameTime gameTime)
        {
            if ((blockingTilesPlaced < maxTilesToPlace && gameClock > 4) || (hasReset && blockingTilesPlaced < maxTilesToPlace))
            {
                showPuzzleCooldown = 1;
                makeConnectingPath();
            }
            //while (maxTilesToPlace > blockingTilesPlaced && fallBackCounter < 200)
            //{
            //    makeConnectingPath();
            //    fallBackCounter++;
            //}

            chooseStartingAndEnding();

            if (isShowingWinningAnimation == false)
            {
                userControls();
                movement();
            }

            searchGrid();

            if (hasDoneOneTimeCode == false)
            {
                fillBorder();
                pathFindingX = startingSquareX;
                pathFindingY = startingSquareY;
                pathDirectionDecider = rand.Next(1, 5);
                //while (maxTilesToPlace > blockingTilesPlaced)
                //{
                //    makeConnectingPath();
                //}
                backgroundCharacterGrid[endingSquareX, endingSquareY].setIsBlockingTile(false);
                //pathDirectionDecider = rand.Next(1, 5);


                hasDoneOneTimeCode = true;
            }

            detectEndingCollision();

            if (cooldownTimer > 0)
            {
                cooldownTimer--;
                if (cooldownTimer == 0)
                {
                    isShowingWinningAnimation = false;
                }
            }
            gameClock++;
        }

        private void userControls()
        {
            if (kb.IsKeyDown(Keys.R) && oldkb.IsKeyUp(Keys.R))
            {
                fallBackCounter = 0;

                resetGrid();
            }

            if (kb.IsKeyDown(Keys.T) && oldkb.IsKeyUp(Keys.T))
            {
                placeRandomBlocks();
            }

            if (kb.IsKeyDown(Keys.C) && oldkb.IsKeyUp(Keys.C))
            {
                playerHighlightedX = startingSquareX;
                playerHighlightedY = startingSquareY;
            }

            if (kb.IsKeyDown(Keys.H) && oldkb.IsKeyUp(Keys.H))
            {
                if (isShowingAnswer == true)
                {
                    isShowingAnswer = false;

                }
                else
                {
                    isShowingAnswer = true;
                }
            }

            if (isMovingLeft == false && isMovingUp == false && isMovingRight == false && isMovingDown == false)
            {

                if ((kb.IsKeyDown(Keys.W) && oldkb.IsKeyUp(Keys.W)) || (kb.IsKeyDown(Keys.Up) && oldkb.IsKeyUp(Keys.Up)))
                {
                    if (playerHighlightedY != 0)
                    {
                        isMovingUp = true;
                    }

                }

                else if ((kb.IsKeyDown(Keys.D) && oldkb.IsKeyUp(Keys.D)) || (kb.IsKeyDown(Keys.Right) && oldkb.IsKeyUp(Keys.Right)))
                {
                    if (playerHighlightedX != gridXDimension - 1)
                    {
                        isMovingRight = true;
                    }

                }

                else if ((kb.IsKeyDown(Keys.S) && oldkb.IsKeyUp(Keys.S)) || (kb.IsKeyDown(Keys.Down) && oldkb.IsKeyUp(Keys.Down)))
                {
                    if (playerHighlightedY != gridYDimension - 1)
                    {
                        isMovingDown = true;
                    }

                }

                else if ((kb.IsKeyDown(Keys.A) && oldkb.IsKeyUp(Keys.A)) || (kb.IsKeyDown(Keys.Left) && oldkb.IsKeyUp(Keys.Left)))
                {
                    if (playerHighlightedX != 0)
                    {
                        isMovingLeft = true;
                    }

                }
            }

        }

        private void movement()
        {
            if (gameClock % 10 == 0)
            {

                if (isMovingUp)
                {
                    if (playerHighlightedY != 0 && backgroundCharacterGrid[playerHighlightedX, playerHighlightedY - 1].getIsBlockingTile() == false)
                    {
                        playerHighlightedY -= 1;
                    }
                    else
                    {
                        isMovingUp = false;
                    }

                }
                else if (isMovingRight)
                {
                    if (playerHighlightedX != gridXDimension - 1 && backgroundCharacterGrid[playerHighlightedX + 1, playerHighlightedY].getIsBlockingTile() == false)
                    {
                        playerHighlightedX += 1;
                    }
                    else
                    {
                        isMovingRight = false;
                    }

                }
                else if (isMovingDown)
                {
                    if (playerHighlightedY != gridYDimension - 1 && backgroundCharacterGrid[playerHighlightedX, playerHighlightedY + 1].getIsBlockingTile() == false)
                    {
                        playerHighlightedY += 1;
                    }
                    else
                    {
                        isMovingDown = false;
                    }

                }
                else if (isMovingLeft)
                {
                    if (playerHighlightedX != 0 && backgroundCharacterGrid[playerHighlightedX - 1, playerHighlightedY].getIsBlockingTile() == false)
                    {
                        playerHighlightedX -= 1;
                    }
                    else
                    {
                        isMovingLeft = false;
                    }

                }



            }

        }

        private void resetGrid()
        {
            endingSquareX = 0;
            endingSquareY = 0;
            iterationNum = 0;
            solutionCheckerX = 0;
            solutionCheckerY = 0;

            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    backgroundCharacterGrid[x, y].setIsStartingSquare(false);
                    backgroundCharacterGrid[x, y].setIsEndingSquare(false);
                    backgroundCharacterGrid[x, y].setIsBlockingTile(false);
                    backgroundCharacterGrid[x, y].setIsNeededTile(false);
                    backgroundCharacterGrid[x, y].setIterationNum(-1);

                }

            }
            fillBorder();


            isMovingDown = false;
            isMovingLeft = false;
            isMovingUp = false;
            isMovingRight = false;
            hasChosenStartingAndEnding = false;
            hasAssignedStartingValues = false;
            blockingTilesPlaced = 0;
            totalNeededTilesPlaced = 0;

            while (neededBlockingTileYCoords.Count > 0)
            {
                neededBlockingTileYCoords.RemoveAt(0);
            }

            while (neededBlockingTileXCoords.Count > 0)
            {
                neededBlockingTileXCoords.RemoveAt(0);
            }


            //hasDoneOneTimeCode = false;
            hasReset = true;



        }

        private void searchGrid()
        {
            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    if (backgroundCharacterGrid[x, y].getIsStartingSquare())
                    {
                        startingSquareX = x;
                        startingSquareY = y;
                    }
                    else if (backgroundCharacterGrid[x, y].getIsEndingSquare())
                    {
                        endingSquareX = x;
                        endingSquareY = y;
                    }


                }

            }

            if (hasAssignedStartingValues == false)
            {
                playerHighlightedX = startingSquareX;
                playerHighlightedY = startingSquareY;
                pathFindingX = startingSquareX;
                pathFindingY = startingSquareY;
                hasAssignedStartingValues = true;

            }

        }

        private void chooseStartingAndEnding()
        {
            if (hasChosenStartingAndEnding == false)
            {

                directionDecider = rand.Next(1, 5);
                tempDecider = directionDecider;

                startingSquareX = rand.Next(1, gridXDimension - 1);
                startingSquareY = rand.Next(1, gridYDimension - 1);
                while (startingSquareY == 1 || startingSquareY == gridYDimension - 2 || startingSquareX == 1 ||
                    startingSquareX == gridXDimension - 2 || (startingSquareX == 0 && startingSquareY == 0) ||
                    (startingSquareX == 0 && startingSquareY == gridYDimension - 1) ||
                    (startingSquareX == gridXDimension - 1 && startingSquareY == 0) ||
                        (startingSquareX == gridXDimension - 1 && startingSquareY == gridYDimension - 1))
                {

                    startingSquareX = rand.Next(0, gridXDimension);
                    //Console.WriteLine(testCounter);
                    startingSquareY = rand.Next(0, gridYDimension);

                    //backgroundCharacterGrid[startingSquareX, startingSquareY].setIsStartingSquare(true);
                    //Console.WriteLine("testCounter: " + testCounter);

                }

                //might be needed


                backgroundCharacterGrid[startingSquareX, startingSquareY].setIsStartingSquare(true);
                //backgroundCharacterGrid[startingSquareX, startingSquareY].setIsBlockingTile(true);


                //endingSquareX = 0;
                //endingSquareY = 0;

                //endingSquareX = rand.Next(0, gridXDimension);
                //endingSquareY = rand.Next(0, gridYDimension);

                ////ending square code

                //while (endingSquareX == 1 || endingSquareX == gridXDimension - 2 || endingSquareY == 1 || endingSquareY == gridYDimension - 2 ||
                //    endingSquareX == startingSquareX || endingSquareY == startingSquareY || (endingSquareX == 0 && endingSquareY == 0) ||
                //    (endingSquareX == 0 && endingSquareY == gridYDimension - 1) || (endingSquareX == gridXDimension - 1 && endingSquareY == 0) ||
                //    (endingSquareX == gridXDimension - 1 && endingSquareY == gridYDimension - 1))
                //{
                //    endingSquareX = rand.Next(0, gridXDimension);
                //    endingSquareY = rand.Next(0, gridYDimension);
                //}

                backgroundCharacterGrid[endingSquareX, endingSquareY].setIsEndingSquare(true);

                hasChosenStartingAndEnding = true;

                fillBorder();
                hasAssignedStartingValues = false;
                searchGrid();
            }

        }

        private void doubleCheckSolution()
        {
            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    if (solutionCheckerX == 0 && solutionCheckerY == 0 && backgroundCharacterGrid[x, y].getIsEndingSquare() == true)
                    {
                        solutionCheckerX = x;
                        solutionCheckerY = y;
                    }

                    if (backgroundCharacterGrid[x - 1, y].getIsStartingSquare() == false && backgroundCharacterGrid[x + 1, y].getIsStartingSquare() == false &&
                        backgroundCharacterGrid[x, y + 1].getIsStartingSquare() == false && backgroundCharacterGrid[x, y - 1].getIsStartingSquare() == false)
                    {
                        if (backgroundCharacterGrid[x - 1, y].getIsNeededTile() == false && backgroundCharacterGrid[x + 1, y].getIsNeededTile() == false &&
                        backgroundCharacterGrid[x, y + 1].getIsNeededTile() == false && backgroundCharacterGrid[x, y - 1].getIsNeededTile() == false)
                        {
                            resetGrid();

                        }




                        if (backgroundCharacterGrid[x - 1, y].getIsNeededTile() == true)
                        {
                            solutionCheckerX = x - 1;
                            lastDirectionDoubleChecked = 4;
                        }
                        else if (backgroundCharacterGrid[x + 1, y].getIsNeededTile() == true)
                        {
                            solutionCheckerX = x + 1;
                            lastDirectionDoubleChecked = 2;
                        }
                        else if (backgroundCharacterGrid[x, y-1].getIsNeededTile() == true)
                        {
                            solutionCheckerY = y - 1;
                            lastDirectionDoubleChecked = 1;
                        }
                        else if (backgroundCharacterGrid[x, y + 1].getIsNeededTile() == true)
                        {
                            solutionCheckerY = y + 1;
                            lastDirectionDoubleChecked = 3;
                        }

                    }

                }

            }
        }

        private void fillBorder()
        {
            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    if (x == 0 || y == 0 || x == gridXDimension - 1 || y == gridYDimension - 1)
                    {
                        if (x != endingSquareX || y != endingSquareY)
                        {
                            backgroundCharacterGrid[x, y].setIsBlockingTile(true);
                        }
                    }
                }

            }

        }

        private void checkForLongConnections()
        {
            horizontalBlockChain = 0;
            verticalBlockChain = 0;
            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    if (backgroundCharacterGrid[x, y].getIsNeededTile() == true)
                    {
                        for (int k = 0; k < gridXDimension; k++)
                        {
                            if (x + k < gridXDimension && backgroundCharacterGrid[x + k, y].getIsNeededTile() == true)
                            {
                                horizontalBlockChain += 1;
                            }
                            else
                            {
                                k = gridXDimension - 1;
                            }
                        }

                        for (int m = 0; m < gridYDimension; m++)
                        {
                            if (y+m < gridYDimension && backgroundCharacterGrid[x, y + m].getIsNeededTile() == true)
                            {
                                verticalBlockChain += 1;
                            }
                            else
                            {
                                m = gridYDimension - 1;
                            }
                        }
                    }

                    if (horizontalBlockChain > 8 || verticalBlockChain > 8)
                    {
                        resetGrid();
                        y = gridYDimension - 1;
                        x = gridXDimension - 1;

                    }

                    verticalBlockChain = 0;
                    horizontalBlockChain = 0;

                }

            }

        }

        private void placeRandomBlocks()
        {
            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    if (backgroundCharacterGrid[x, y].getIsStartingSquare() == false && backgroundCharacterGrid[x, y].getIsNeededTile() == false &&
                        backgroundCharacterGrid[x, y].getIsEndingSquare() == false && backgroundCharacterGrid[x, y].getIsBlockingTile() == false)
                    {
                        randomDecider = rand.Next(1, 6);

                        if (randomDecider == 1)
                        {
                            backgroundCharacterGrid[x, y].setIsBlockingTile(true);
                        }
                    }

                }

            }

        }

        private void makeConnectingPath()
        {


            if (shouldReset == true)
            {
                resetGrid();
                shouldReset = false;

            }
            pathDirectionDecider = rand.Next(1, 3);
            numTilesToMove = rand.Next(3, 8);

            if (pathDirectionDecider == 1 && oldPathFindingDirection != 1)
            {
                pathDirectionDecider = rand.Next(1, 3);
                if (pathDirectionDecider == 1 && pathFindingY - numTilesToMove > 0)
                {
                    for (int i = 0; i < numTilesToMove; i++)
                    {
                        pathFindingY -= 1;
                        if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsBlockingTile() == false &&
                            backgroundCharacterGrid[pathFindingX, pathFindingY].getIsStartingSquare() == false)
                        {
                            backgroundCharacterGrid[pathFindingX, pathFindingY].setIsNeededTile(true);

                            totalNeededTilesPlaced += 1;
                            backgroundCharacterGrid[pathFindingX, pathFindingY].setIterationNum(iterationNum);
                            if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsStartingSquare() == true)
                            {
                                resetGrid();
                                shouldReset = true;
                            }
                        }
                        else
                        {
                            resetGrid();
                        }
                    }
                    if (backgroundCharacterGrid[pathFindingX, pathFindingY - 1].getIsNeededTile() == false)
                    {
                        backgroundCharacterGrid[pathFindingX, pathFindingY - 1].setIsBlockingTile(true);
                        neededBlockingTileYCoords.Add(pathFindingY - 1);
                        neededBlockingTileXCoords.Add(pathFindingX);
                    }
                    else
                    {
                        resetGrid();
                        shouldReset = true;

                    }
                    oldPathFindingDirection = 1;
                    blockingTilesPlaced += 1;
                    iterationNum++;

                }
                else if (pathDirectionDecider == 2 && pathFindingY + numTilesToMove < gridYDimension - 1)
                {
                    for (int i = 0; i < numTilesToMove; i++)
                    {
                        pathFindingY += 1;
                        if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsBlockingTile() == false &&
                            backgroundCharacterGrid[pathFindingX, pathFindingY].getIsStartingSquare() == false)
                        {
                        backgroundCharacterGrid[pathFindingX, pathFindingY].setIterationNum(iterationNum);
                        totalNeededTilesPlaced += 1;

                        backgroundCharacterGrid[pathFindingX, pathFindingY].setIsNeededTile(true);
                        totalNeededTilesPlaced += 1;

                        if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsStartingSquare() == true)
                        {
                            resetGrid();
                            shouldReset = true;

                        }
                            }
                        else
                        {
                            resetGrid();
                        }
                    }
                    if (backgroundCharacterGrid[pathFindingX, pathFindingY + 1].getIsNeededTile() == false)
                    {
                        backgroundCharacterGrid[pathFindingX, pathFindingY + 1].setIsBlockingTile(true);
                        neededBlockingTileYCoords.Add(pathFindingY + 1);
                        neededBlockingTileXCoords.Add(pathFindingX);


                    }
                    else
                    {
                        resetGrid();
                        shouldReset = true;

                    }
                    oldPathFindingDirection = 1;
                    blockingTilesPlaced += 1;
                    iterationNum++;


                }


            }
            else if (pathDirectionDecider == 2 && oldPathFindingDirection != 2)
            {
                pathDirectionDecider = rand.Next(1, 3);
                if (pathDirectionDecider == 1 && pathFindingX - numTilesToMove > 0)
                {
                    for (int i = 0; i < numTilesToMove; i++)
                    {
                        pathFindingX -= 1;
                        if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsBlockingTile() == false &&
                            backgroundCharacterGrid[pathFindingX, pathFindingY].getIsStartingSquare() == false)
                        {
                            backgroundCharacterGrid[pathFindingX, pathFindingY].setIterationNum(iterationNum);
                            backgroundCharacterGrid[pathFindingX, pathFindingY].setIsNeededTile(true);
                            totalNeededTilesPlaced += 1;

                            if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsStartingSquare() == true)
                            {
                                resetGrid();
                                shouldReset = true;

                            }
                        }
                        else
                        {
                            resetGrid();
                        }
                    }
                    if (backgroundCharacterGrid[pathFindingX - 1, pathFindingY].getIsNeededTile() == false)
                    {
                        backgroundCharacterGrid[pathFindingX - 1, pathFindingY].setIsBlockingTile(true);
                        neededBlockingTileXCoords.Add(pathFindingX);
                        neededBlockingTileYCoords.Add(pathFindingY);

                    }
                    else
                    {
                        resetGrid();
                        shouldReset = true;

                    }
                    blockingTilesPlaced += 1;
                    oldPathFindingDirection = 2;
                    iterationNum++;



                }
                else if (pathDirectionDecider == 2 && pathFindingX + numTilesToMove < gridXDimension - 1)
                {
                    for (int i = 0; i < numTilesToMove; i++)
                    {
                        pathFindingX += 1;
                        if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsBlockingTile() == false &&
                            backgroundCharacterGrid[pathFindingX, pathFindingY].getIsStartingSquare() == false)
                        {
                            backgroundCharacterGrid[pathFindingX, pathFindingY].setIterationNum(iterationNum);
                            backgroundCharacterGrid[pathFindingX, pathFindingY].setIsNeededTile(true);
                            totalNeededTilesPlaced += 1;

                            if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsStartingSquare() == true)
                            {
                                resetGrid();
                                shouldReset = true;

                            }
                        }
                        else
                        {
                            resetGrid();
                        }
                    }
                    if (backgroundCharacterGrid[pathFindingX + 1, pathFindingY].getIsNeededTile() == false)
                    {
                        backgroundCharacterGrid[pathFindingX + 1, pathFindingY].setIsBlockingTile(true);
                        neededBlockingTileXCoords.Add(pathFindingX + 1);
                        neededBlockingTileYCoords.Add(pathFindingY);

                    }
                    else
                    {
                        resetGrid();
                        shouldReset = true;

                    }
                    oldPathFindingDirection = 2;
                    blockingTilesPlaced += 1;
                    iterationNum++;



                }


            }

            checkForLongConnections();

            if (maxTilesToPlace == blockingTilesPlaced)
            {
                //for (int y = 0; y < gridYDimension; y++)
                //{
                //    for (int x = 0; x < gridXDimension; x++)
                //    {
                //        if()

                //    }

                //}

                //if(tot)


                //if (backgroundCharacterGrid[pathFindingX, pathFindingY].getIsNeededTile() == false)
                //{
                backgroundCharacterGrid[pathFindingX, pathFindingY].setIsEndingSquare(true);
                //}
                //else
                //{
                //    resetGrid();
                //}

                pathFindingX = 0;
                pathFindingY = 0;
                endingSquareX = pathFindingX;
                endingSquareY = pathFindingY;
                showPuzzleCooldown = 0;

                placeRandomBlocks();

            }

            tilesSurroundingPathFinder = 0;

            if (pathFindingX != 0 && backgroundCharacterGrid[pathFindingX - 1, pathFindingY].getIsBlockingTile() == true)
            {
                tilesSurroundingPathFinder += 1;
            }
            if (pathFindingX + 1 != gridXDimension && backgroundCharacterGrid[pathFindingX + 1, pathFindingY].getIsBlockingTile() == true)
            {
                tilesSurroundingPathFinder += 1;
            }
            if (pathFindingY != 0 && backgroundCharacterGrid[pathFindingX, pathFindingY - 1].getIsBlockingTile() == true)
            {
                tilesSurroundingPathFinder += 1;
            }
            if (pathFindingY + 1 != gridYDimension && backgroundCharacterGrid[pathFindingX, pathFindingY + 1].getIsBlockingTile() == true)
            {
                tilesSurroundingPathFinder += 1;
            }

            //if (tilesSurroundingPathFinder == 4)
            //{
            //    resetGrid();
            //}

            //}

            //Console.WriteLine("blockingTilesPlaced: " + blockingTilesPlaced);



        }

        private void detectEndingCollision()
        {
            if (playerHighlightedX == endingSquareX && playerHighlightedY == endingSquareY)
            {
                isShowingWinningAnimation = true;
                cooldownTimer += 90;
                resetGrid();

            }

        }

        private void placeOnSide(int sideNum)
        {


        }

        private void drawTitleScreen()
        {


        }

        private void drawGamePlay(GameTime gameTime)
        {
            backgroundOutline.DrawCharacter(spriteBatch);
            //backgroundCharacterGrid[playerHighlightedX, playerHighlightedY].DrawCharacter(spriteBatch);

            for (int y = 0; y < gridYDimension; y++)
            {
                for (int x = 0; x < gridXDimension; x++)
                {
                    if (showPuzzleCooldown == 0)
                    {

                        if (backgroundCharacterGrid[x, y].getIsBlockingTile() == true)
                        {
                            backgroundCharacterGrid[x, y].DrawCharacter(spriteBatch, Color.Black);
                        }
                        else if (backgroundCharacterGrid[x, y].getIsNeededTile() == true && isShowingAnswer == true)
                        {
                            backgroundCharacterGrid[x, y].DrawCharacter(spriteBatch, Color.Aquamarine);
                        }
                        else
                        {
                            backgroundCharacterGrid[x, y].DrawCharacter(spriteBatch);
                        }

                    }
                    if (isShowingAnswer == true)
                    {
                        //spriteBatch.DrawString(customfont, x + "," + y, new Vector2(backgroundCharacterGrid[x, y].GetRekt.Center.X - (screenWidth / (gridXDimension * 4)),
                        //    backgroundCharacterGrid[x, y].GetRekt.Center.Y), Color.Orange);

                        spriteBatch.DrawString(customfont, backgroundCharacterGrid[x, y].getIterationNum().ToString(), new Vector2(backgroundCharacterGrid[x, y].GetRekt.Center.X - (screenWidth / (gridXDimension * 4)),
                            backgroundCharacterGrid[x, y].GetRekt.Center.Y), Color.Orange);

                    }
                }

            }

            //if (isShowingWinningAnimation == true)
            //{
            //    if (gameClock == 179)
            //    {
            //        playerHighlightedX = 1;
            //        playerHighlightedY = 1;
            //    }
            //    if (gameClock % 5 == 0)
            //    {
            //        if (isAnimationMovingHorizontal)
            //        {
            //            playerHighlightedX += amountToAddToX;
            //        }
            //        if (isAnimationMovingVertical)
            //        {
            //            playerHighlightedY += amountToAddToY;
            //        }

            //        if (playerHighlightedX == gridXDimension - 2 && isAnimationMovingHorizontal == true)
            //        {
            //            isAnimationMovingVertical = true;
            //            isAnimationMovingHorizontal = false;
            //        }

            //        if (playerHighlightedX == 1 && amountToAddToX == -1)
            //        {
            //            isAnimationMovingVertical = true;
            //            isAnimationMovingHorizontal = false;
            //            amountToAddToX = 0;
            //            amountToAddToY = -1;
            //        }

            //        if (isAnimationMovingVertical == true)
            //        {
            //            if (amountToAddToX > 0)
            //            {
            //                amountToAddToY = 1;
            //            }
            //            else
            //            {
            //                amountToAddToY = -1;
            //            }
            //        }


            //        if (playerHighlightedY == gridYDimension - 2 && isAnimationMovingVertical == true)
            //        {
            //            isAnimationMovingVertical = false;
            //            isAnimationMovingHorizontal = true;
            //            amountToAddToX = -(amountToAddToX);
            //        }
            //    }


            //}
            //else

            //{
            if (showPuzzleCooldown == 0)
            {
                backgroundCharacterGrid[startingSquareX, startingSquareY].DrawCharacter(spriteBatch, Color.Green);
                backgroundCharacterGrid[endingSquareX, endingSquareY].DrawCharacter(spriteBatch, Color.Purple);
                backgroundCharacterGrid[pathFindingX, pathFindingY].DrawCharacter(spriteBatch, Color.YellowGreen);
            }
            //}

            if (playerHighlightedX != startingSquareX || playerHighlightedY != startingSquareY)
            {
                backgroundCharacterGrid[playerHighlightedX, playerHighlightedY].DrawCharacter(spriteBatch, Color.Red);
            }

            //spriteBatch.DrawString(customfont, "playerHighlightedX: " + playerHighlightedX, new Vector2(400, 350), Color.Blue);
            //spriteBatch.DrawString(customfont, "playerHighlightedY: " + playerHighlightedY, new Vector2(400, 370), Color.Blue);

            //spriteBatch.DrawString(customfont, "endingX: " + endingSquareX, new Vector2(400, 350), Color.Blue);
            //spriteBatch.DrawString(customfont, "endingY: " + endingSquareY, new Vector2(400, 370), Color.Blue);

            //spriteBatch.DrawString(customfont, "startingSquareX: " + startingSquareX, new Vector2(480, 350), Color.Blue);
            //spriteBatch.DrawString(customfont, "startingSquareY: " + startingSquareY, new Vector2(480, 370), Color.Blue);


        }

        private void drawOptionsScreen()
        {


        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here


            spriteBatch.Begin();

            switch (state)
            {

                case gameState.titleScreen:
                    drawTitleScreen();
                    break;
                case gameState.gamePlay:

                    drawGamePlay(gameTime);
                    break;

                case gameState.options:

                    drawOptionsScreen();

                    break;

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
