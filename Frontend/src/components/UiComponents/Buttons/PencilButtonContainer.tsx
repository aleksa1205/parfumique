export default function PencilButtonContainer({children, className}: {
    children: any;
    className?: string;
}) {
    return (
        <div className={`flex justify-between items-center gap-4 ${className}`}>
            {children}
        </div>
    )
}