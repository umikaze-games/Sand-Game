# Sand Simulation Game

A **Unity-based sand simulation & puzzle**. Players drag randomly generated shapes into a sandbox; sand grains fall under gravity, and when a same-color region connects **right → left**, that region is cleared. :contentReference[oaicite:0]{index=0}:contentReference[oaicite:1]{index=1}

---

## ✨ Features
- **Sand physics**: grid-based falling/diagonal sliding with border-aware behavior. :contentReference[oaicite:2]{index=2}
- **Drag & Drop** with smooth lerp, raycast picking, and boundary clamping. :contentReference[oaicite:3]{index=3}
- **Right-to-left match clear** using BFS connected-region detection. :contentReference[oaicite:4]{index=4}
- **Sprite → Shape pipeline**: convert sprites to `Cell[,]` shapes and recolor at runtime. :contentReference[oaicite:5]{index=5}:contentReference[oaicite:6]{index=6}:contentReference[oaicite:7]{index=7}
- **Modular data types**: `Cell`, `EMaterialType`, `Shape`, `ShapeHolder`, `ShapeManager`. :contentReference[oaicite:8]{index=8}:contentReference[oaicite:9]{index=9}:contentReference[oaicite:10]{index=10}:contentReference[oaicite:11]{index=11}

---

## 📷 Demo
> *(Optional)* Add a GIF or screenshot here:
>
> `![Sand Simulation](docs/sand-sim.gif)`

---

## 🕹️ How to Play
1. Pick a shape from the slots and **drag** it into the sandbox. :contentReference[oaicite:12]{index=12}:contentReference[oaicite:13]{index=13}
2. **Release** to drop; sand settles under gravity each frame. :contentReference[oaicite:14]{index=14}
3. When any color makes a **continuous path from the rightmost column to the leftmost column**, that region is **cleared**. :contentReference[oaicite:15]{index=15}
4. After every **3 drops**, new shapes repopulate the slots. :contentReference[oaicite:16]{index=16}
