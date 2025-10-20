import { createContext, useContext, useEffect, useState, useRef, useCallback } from "react";

import type { Stroke, Point } from "../types/stroke";
import { BoardWebSocket } from "../services/boardWebSocket";

type DrawContextType = {
    drawMode: "paint" | "erase";
    setDrawMode: React.Dispatch<React.SetStateAction<"paint" | "erase">>;
    strokes: Stroke[];
    color: string;
    setColor: React.Dispatch<React.SetStateAction<string>>;
    lineWidth: number;
    setLineWidth: React.Dispatch<React.SetStateAction<number>>;
    addStroke: (stroke: Stroke) => void;
    addPointToStroke: (id: string, point: Point) => void;
    clearBoard: () => void;
}

const DrawContext = createContext<DrawContextType | undefined>(undefined);

interface DrawProviderProps {
    children: React.ReactNode;
    boardId: string;
}

export const DrawProvider: React.FC<DrawProviderProps> = ({ children, boardId }) => {
    const [drawMode, setDrawMode] = useState<"paint" | "erase">("paint");
    const [strokes, setStrokes] = useState<Stroke[]>([]);
    const [color, setColor] = useState<string>("black");
    const [lineWidth, setLineWidth] = useState<number>(2);
    const webSocketRef = useRef<BoardWebSocket | null>(null);

    useEffect(() => {
        const webSocket = new BoardWebSocket(boardId, {
            onStrokeAdded: (stroke) => addStroke(stroke, "remote"),
            onPointAddedToStroke: (strokeId, point) => addPointToStroke(strokeId, point, "remote"),
            onClearBoard: () => clearBoard("remote"),
        });

        webSocketRef.current = webSocket;
        webSocket.connect();

        return () => webSocket.disconnect();
    }, [boardId])

    const addStroke = useCallback((stroke: Stroke, source: "local" | "remote" = "local") => {
        setStrokes((prev) => [...prev, stroke]);

        if (source === "local") {
            webSocketRef.current?.sendAddStroke(stroke);
        }
    }, []);

    const addPointToStroke = useCallback((id: string, point: Point, source: "local" | "remote" = "local") => {
        setStrokes((prev) => 
            prev.map(stroke => 
                stroke.id === id ? { ...stroke, points: [...stroke.points, point] } : stroke
            )
        );

        if (source === "local") {
            webSocketRef.current?.sendAddPointToStroke(id, point);
        }
    }, []);

    const clearBoard = useCallback((source: "local" | "remote" = "local") => {
        setStrokes([]);

        if (source === "local") {
            webSocketRef.current?.sendClearBoard();
        }
    }, []);

    const value: DrawContextType = {
        drawMode,
        setDrawMode,
        strokes, 
        color, 
        setColor, 
        lineWidth, 
        setLineWidth, 
        addStroke, 
        addPointToStroke,
        clearBoard,
    }

    return (
        <DrawContext.Provider value={value}>
            { children }
        </DrawContext.Provider>
    );
}

export function useDrawContext() {
    const context = useContext(DrawContext);
    if (!context) {
        throw new Error("useDrawContext must be used within DrawProvider.");
    }
    return context;
}
