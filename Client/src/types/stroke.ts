
export type Point  = {
    x: number; y: number;
}

export type Stroke = {
    id: string;
    userId: string | undefined;
    color: string;
    lineWidth: number;
    points: Point[];
}
