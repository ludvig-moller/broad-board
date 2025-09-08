import { useParams } from "react-router-dom";
import DrawCanvas from "../components/DrawCanvas";

function Board() {
    const { boardId } = useParams();

    return (
        <>
            <h2>{boardId}</h2>
            <DrawCanvas />
        </>
    );
}

export default Board;