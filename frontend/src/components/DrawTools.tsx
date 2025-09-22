import { useDrawContext } from "../context/DrawContext";

function DrawTools() {
    const { setColor, setLineWidth } = useDrawContext();

    return (
        <aside>
            <div>
                <button onClick={() => setColor("black")}>Black</button>
                <button onClick={() => setColor("red")}>Red</button>
                <button onClick={() => setColor("green")}>Green</button>
                <button onClick={() => setColor("blue")}>Blue</button>
            </div>
            <input type="range" min={2} max={10} defaultValue={2} onChange={(e: React.ChangeEvent<HTMLInputElement>) => setLineWidth(Number(e.target.value))}/>
        </aside>
    );
}

export default DrawTools;
