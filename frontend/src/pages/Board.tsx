import { useParams } from "react-router-dom";

function Board() {
    const { boardId } = useParams();

    return (
        <h1>Board: {boardId}</h1>
    );
}

export default Board;