
interface Props {
    setStrokeColor: React.Dispatch<React.SetStateAction<string>>;
    setLineWidth: React.Dispatch<React.SetStateAction<number>>;
    clearCanvas: () => void;
    undoLastCanvasUpdate: () => void;
}

function DrawTools(props: Props) {
    return (
        <aside>
            <div>
                <button onClick={() => props.setStrokeColor("black")}>Black</button>
                <button onClick={() => props.setStrokeColor("red")}>Red</button>
                <button onClick={() => props.setStrokeColor("green")}>Green</button>
                <button onClick={() => props.setStrokeColor("blue")}>Blue</button>
            </div>
            <input type="range" min={2} max={10} defaultValue={2} onChange={(e) => props.setLineWidth(Number(e.target.value))}/>
            <button onClick={props.clearCanvas}>Clear</button>
            <button onClick={props.undoLastCanvasUpdate}>Undo</button>
        </aside>
    );
}

export default DrawTools;
