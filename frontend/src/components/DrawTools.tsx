import { useDrawContext } from "../context/DrawContext";

export default function DrawTools() {
    const { drawMode, setDrawMode, color, setColor, setLineWidth, clearBoard } = useDrawContext();
    const colors = ["black", "red", "green", "blue"];

    return (
        <aside className="draw-tools-container">
            <div className="draw-mode-container">
                <button className={`btn ${drawMode === "paint" ? "active" : ""}`} onClick={() => setDrawMode("paint")}>Paint</button>
                <button className={`btn ${drawMode === "erase" ? "active" : ""}`} onClick={() => setDrawMode("erase")}>Erase</button>
            </div>
            <div className="color-container">
                {colors.map((c) => (
                    <button 
                    key={c}
                    className={`btn color ${c} ${color === c ? "active" : ""}`} 
                    onClick={() => setColor(c)} />
                ))}
            </div>
            <input
                className="slider line-width" 
                type="range" 
                min={2} 
                max={10} 
                defaultValue={2} 
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => setLineWidth(Number(e.target.value))}
            />
            <button className="btn clear-board" onClick={() => clearBoard()}>Clear</button>
        </aside>
    );
}
