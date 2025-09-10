import { useState } from "react";
import { useParams } from "react-router-dom";
import DrawCanvas from "../components/DrawCanvas";
import DrawTools from "../components/DrawTools";

function Board() {
    const { boardId } = useParams();

    const [strokeColor, setStrokeColor] = useState("black");
    const [lineWidth, setLineWidth] = useState(2);
    const [clearCanvas, setClearCanvas] = useState<() => void>(() => {});
    const [undoLastCanvasUpdate, setUndoLastCanvasUpdate] = useState<() => void>(() => {});

    return (
        <>
            <h2>{boardId}</h2>
            <DrawCanvas 
                strokeColor={strokeColor}
                lineWidth={lineWidth}
                setClearCanvas={setClearCanvas}
                setUndoLastCanvasUpdate={setUndoLastCanvasUpdate}
            />
            <DrawTools 
                setStrokeColor={setStrokeColor}
                setLineWidth={setLineWidth}
                clearCanvas={clearCanvas}
                undoLastCanvasUpdate={undoLastCanvasUpdate}
            />
        </>
    );
}

export default Board;