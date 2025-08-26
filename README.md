# Sand Simulation Game

A **Unity-based sand simulation & puzzle**. Players drag randomly generated shapes into a sandbox; sand grains fall under gravity, and when a same-color region connects **right → left**, that region is cleared.

---

## ✨ Features
- **Sand physics**: grid-based falling and diagonal sliding with border-aware behavior.
- **Drag & Drop**: smooth mouse dragging with raycast picking and boundary clamping.
- **Right-to-left match clear**: BFS-based connected-region detection for clearing blocks.
- **Sprite → Shape pipeline**: automatically converts sprites into `Cell[,]` shapes and recolors them at runtime.
- **Modular data types**: `Cell`, `EMaterialType`, `Shape`, `ShapeHolder`, `ShapeManager`.

---

## 📷 Demo
> *(Optional)* Add a GIF or screenshot here:
>
![Sand Simulation Demo](docs/sand-sim.gif)

---

## 🕹️ How to Play
1. Pick a shape from the slots and **drag** it into the sandbox.  
2. **Release** to drop; sand settles under gravity each frame.  
3. When any color makes a **continuous path from the rightmost column to the leftmost column**, that region is **cleared**.  
4. After every **3 drops**, new shapes repopulate the slots.  
