import { useParams } from "react-router-dom";
import { DrawProvider } from "../context/DrawContext";
import DrawCanvas from "../components/DrawCanvas";
import DrawTools from "../components/DrawTools";

function Board() {
    const { boardId } = useParams();

    if (!boardId) return; // Redirect to home page?

    return (
        <>
            <h2>{boardId}</h2>
            <DrawProvider boardId={boardId}>
                <DrawCanvas />
                <DrawTools />
            </DrawProvider>
        </>
    );
}

export default Board;