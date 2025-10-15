import { useParams, useNavigate } from "react-router-dom";
import { DrawProvider } from "../context/DrawContext";
import DrawCanvas from "../components/DrawCanvas";
import DrawTools from "../components/DrawTools";

function Board() {
    const { boardId } = useParams();

    const navigate = useNavigate();
    if (!boardId) return navigate("/");

    return (
        <>
            <DrawProvider boardId={boardId}>
                <DrawCanvas />
                <DrawTools />
            </DrawProvider>
        </>
    );
}

export default Board;