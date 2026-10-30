# Market Clash – Turn-Based Strategy Game

A small turn-based market strategy game developed in Unity as part of the Unity Game Developer Machine Test.

The game features a human player competing against an AI opponent in a simulated stock market. Players make strategic decisions by buying, selling, or passing on shares while using private market cards to anticipate price movements.

## 🎮 Project Information

| Detail | Information |
|---|---|
| Project | Market Clash |
| Engine | Unity |
| Unity Version | 6.3.7f1 |
| Language | C# |
| Platform | Windows / Unity Editor |
| Game Type | Turn-Based Strategy |

## 🕹️ Game Overview

- 1 Human Player vs 1 AI Opponent.
- 3 Fictional Companies:
  - Nova Tech
  - Green Energy
  - Mega Foods
- 3 Rounds of gameplay.
- Each player starts with ₹1,000 cash and 0 shares.
- Each player gets 3 actions per round.

### Gameplay Flow

1. **Round Start:** Players receive their private market cards.
2. **Player Turn:** The human player can buy shares, sell shares, or pass.
3. **AI Turn:** The AI opponent makes decisions based on its market cards and current holdings.
4. **Market Reveal:** Both players' market cards are revealed, and company prices are updated.
5. **Settlement:** Player portfolios and net worth are recalculated.
6. **Next Round:** New cards are distributed while cash, holdings, and market prices carry forward.
7. **Game Over:** The player with the highest final net worth wins.

### Starting Market Prices

| Company | Starting Price |
|---|---:|
| Nova Tech | ₹50 |
| Green Energy | ₹40 |
| Mega Foods | ₹30 |

## 🃏 Market Card System

Each player receives 3 private market cards at the beginning of every round.

Cards represent positive or negative price movements for specific companies.

The market price is calculated using:

`New Price = Current Price + Total Revealed Card Movements`

The minimum company share price is ₹5.

## 🤖 AI Opponent

The computer opponent uses market card information and its current portfolio to make trading decisions.

Its decision-making considerations include:
- Prioritizing companies associated with positive market cards.
- Considering selling shares associated with negative market movements.
- Validating available cash before purchasing shares.
- Passing when no valid trading action is available.

## 🏗️ Architecture

The project uses C# scripts and Unity components to handle different gameplay responsibilities.

The main gameplay responsibilities include:

- **Game Management:** Controls the overall game flow and state transitions.
- **Turn Management:** Handles player and AI turns and action counts.
- **Market Management:** Maintains company prices and calculates market movements.
- **Player Management:** Handles cash, share holdings, buying, and selling.
- **AI Management:** Handles computer opponent decisions.
- **Card Management:** Handles market card distribution and reveal logic.
- **UI Management:** Updates gameplay information and visual feedback.

The current implementation focuses on delivering functional gameplay within the machine test timeframe. Some areas of the architecture could benefit from further refactoring to improve modularity and reduce coupling.

## 🚀 How to Run

1. Clone the repository:

   ```bash
   git clone https://github.com/Jyotirmoy2003/Market-Clash.git
   ```

2. Open Unity Hub.
3. Select **Add Project from Disk**.
4. Select the cloned project folder.
5. Open the project using **Unity 6.3.7f1**.
6. Locate and open the main gameplay scene.
7. Press the Play button in the Unity Editor.

## ⚙️ Development Notes

Due to the strict 3-hour development timeframe, the primary focus was on implementing the core gameplay functionality and required systems.

Some areas that could be improved with additional development time:

- Further UI/UX refinement and visual polish.
- Additional UI animations and gameplay transitions.
- More comprehensive edge-case testing.
- Refactoring the architecture to make greater use of C# events and delegates.
- Improving communication between gameplay systems through a more loosely coupled event-driven approach.

These improvements would help make the project more modular, maintainable, and scalable.

## 📝 Assumptions

- The game follows the rules and gameplay structure provided in the machine test document.
- The AI opponent operates using its own private market cards.
- Invalid transactions should not consume a player's available actions.
- Company prices cannot fall below ₹5.
- Final results are determined by comparing both players' calculated net worth.

## 👨‍💻 Developer

**Jyotirmoy Mondal**

Unity Game Developer

GitHub: [Jyotirmoy2003](https://github.com/Jyotirmoy2003)

Portfolio: [jyotirmoy-portfolio.netlify.app](https://jyotirmoy-portfolio.netlify.app/)

---

*Developed as part of the Market Clash Unity Game Developer Machine Test.*
