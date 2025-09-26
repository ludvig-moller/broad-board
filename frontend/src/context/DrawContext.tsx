import { createContext, useContext, useEffect, useState, useRef, useCallback } from "react";
import type { Stroke, Point } from "../types/stroke";
import { BoardWebSocket } from "../services/boardWebSocket";

type DrawContextType = {
    strokes: Stroke[];
    color: string;
    setColor: React.Dispatch<React.SetStateAction<string>>;
    lineWidth: number;
    setLineWidth: React.Dispatch<React.SetStateAction<number>>;
    addStroke: (stroke: Stroke) => void;
    addPointToStroke: (id: string, point: Point) => void;
}

const DrawContext = createContext<DrawContextType | undefined>(undefined);

interface DrawProviderProps {
    children: React.ReactNode;
    boardId: string;
}

export const DrawProvider: React.FC<DrawProviderProps> = ({ children, boardId }) => {
    const [strokes, setStrokes] = useState<Stroke[]>([]);
    const [color, setColor] = useState<string>("black");
    const [lineWidth, setLineWidth] = useState<number>(2);
    const webSocketRef = useRef<BoardWebSocket | null>(null);

    useEffect(() => {
        const webSocket = new BoardWebSocket(boardId, {
            onStrokeAdded: (stroke) => {
                setStrokes((prev) => [...prev, stroke]);
            },
            onPointAddedToStroke: (strokeId, point) => {
                setStrokes((prev) => 
                    prev.map(stroke => 
                        stroke.id == strokeId ? { ...stroke, points: [...stroke.points, point] } : stroke
                    )
                );
            }
        });

        webSocketRef.current = webSocket;
        webSocket.connect();

        return () => webSocket.disconnect();
    }, [boardId])

    const addStroke = useCallback((stroke: Stroke) => {
        setStrokes((prev) => [...prev, stroke]);

        webSocketRef.current?.sendAddStroke(stroke);
    }, []);

    const addPointToStroke = useCallback((id: string, point: Point) => {
        setStrokes((prev) => 
            prev.map(stroke => 
                stroke.id == id ? { ...stroke, points: [...stroke.points, point] } : stroke
            )
        );

        webSocketRef.current?.sendAddPointToStroke(id, point);
    }, []);

    const value: DrawContextType = {
        strokes, 
        color, 
        setColor, 
        lineWidth, 
        setLineWidth, 
        addStroke, 
        addPointToStroke
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
