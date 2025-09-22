
import { createContext, useContext, useState} from "react";
import type { Stroke, Point } from "../types/stroke";

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

export const DrawProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [strokes, setStrokes] = useState<Stroke[]>([]);
    const [color, setColor] = useState<string>("black");
    const [lineWidth, setLineWidth] = useState<number>(2);

    const addStroke = (stroke: Stroke) => {
        setStrokes((prev) => [...prev, stroke]);
    };

    const addPointToStroke = (id: string, point: Point) => {
        setStrokes((prev) => 
            prev.map(stroke => 
                stroke.id == id ? { ...stroke, points: [...stroke.points, point] } : stroke
            )
        );
    };

    return (
        <DrawContext.Provider value={{ strokes, color, setColor, lineWidth, setLineWidth, addStroke, addPointToStroke }}>
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
