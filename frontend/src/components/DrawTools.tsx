import { useDrawContext } from "../context/DrawContext";

export default function DrawTools() {
    const { color, setColor, setLineWidth, clearBoard } = useDrawContext();
    const colors = ["black", "red", "green", "blue"];

    return (
        <aside className="draw-tools-container">
            <div className="color-container">
                {colors.map((c) => (
                    <button 
                    key={c}
                    className={`btn color ${c} ${color === c ? "active" : ""}`} 
                    onClick={() => setColor(c)} />
                ))}
            </div>
            <input
                className="slider" 
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
