
export type Point  = {
    x: number; y: number;
}

export type Stroke = {
    id: string;
    color: string;
    lineWidth: number;
    points: Point[];
}
