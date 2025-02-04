export default function ScrollContainer({children}: {
    children: any
}) {
    return (
        <div style={{overflowY: 'scroll', height: window.innerWidth < 1000 ? '400px' : 'auto'}}>
            {children}
        </div>
    )
}