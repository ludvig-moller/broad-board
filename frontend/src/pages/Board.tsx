import { useParams } from "react-router-dom";
import { DrawProvider } from "../context/DrawContext";
import DrawCanvas from "../components/DrawCanvas";

function Board() {
    const { boardId } = useParams();

    return (
        <>
            <h2>{boardId}</h2>
            <DrawProvider>
                <DrawCanvas />
            </DrawProvider>
        </>
    );
}

export default Board;